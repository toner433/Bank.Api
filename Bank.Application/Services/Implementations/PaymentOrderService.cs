using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Application.Common;
using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.PaymentOrders;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;

namespace Bank.Application.Services.Implementations
{
    public class PaymentOrderService : IPaymentOrderService
    {
        private readonly IDataBaseRepository _db;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrganizationService _organizationService;
        private readonly IOperationService _operationService;
        private readonly IElectronicSignatureService _electronicSignatureService;

        public PaymentOrderService(
            IDataBaseRepository db,
            IAccountRepository accountRepository,
            IOrganizationService organizationService,
            IOperationService operationService,
            IElectronicSignatureService electronicSignatureService)
        {
            _db = db;
            _accountRepository = accountRepository;
            _organizationService = organizationService;
            _operationService = operationService;
            _electronicSignatureService = electronicSignatureService;
        }

        public async Task<PaymentOrderDto> CreateAsync(CreatePaymentOrderRequest request, Guid actingUserId)
        {
            if (request.Amount <= 0) throw new BusinessException("Сумма должна быть больше 0");
            if (!string.IsNullOrWhiteSpace(request.RecipientInn) && request.RecipientInn.Trim().Length is < 9 or > 12)
                throw new BusinessException("ИНН получателя должен содержать от 9 до 12 символов");
            if (!string.IsNullOrWhiteSpace(request.RecipientKpp) && request.RecipientKpp.Trim().Length != 9)
                throw new BusinessException("КПП получателя должен содержать 9 символов");
            if (!string.IsNullOrWhiteSpace(request.RecipientBankBik) && request.RecipientBankBik.Trim().Length is < 8 or > 11)
                throw new BusinessException("БИК/SWIFT банка получателя должен содержать от 8 до 11 символов");
            if (!string.IsNullOrWhiteSpace(request.PaymentType) && request.PaymentType.Trim().Length > 40)
                throw new BusinessException("Вид платежа слишком длинный");
            if (!string.IsNullOrWhiteSpace(request.VatType) && request.VatType.Trim().Length > 30)
                throw new BusinessException("Поле НДС заполнено некорректно");
            if (request.PaymentPriority.HasValue && request.PaymentPriority.Value is < 1 or > 5)
                throw new BusinessException("Очередность платежа должна быть в диапазоне от 1 до 5");
            if (request.VatAmount < 0)
                throw new BusinessException("Сумма НДС не может быть отрицательной");

            if (!await _organizationService.UserIsMemberAsync(request.OrganizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var roleOk = await _organizationService.UserIsDirectorAsync(request.OrganizationId, actingUserId)
                || (await MemberRoleAsync(request.OrganizationId, actingUserId)) == OrganizationRoles.Accountant;
            if (!roleOk) throw new BusinessException("Недостаточно прав для создания платёжного поручения");

            var from = await _db.GetByIdAsync<Account>(request.FromAccountId);
            if (from == null) throw new NotFoundException("Счёт не найден");
            if (from.OrganizationId != request.OrganizationId)
                throw new BusinessException("Счёт не принадлежит организации");

            var order = new PaymentOrder
            {
                Id = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                FromAccountId = request.FromAccountId,
                DocumentNumber = string.IsNullOrWhiteSpace(request.DocumentNumber) ? null : request.DocumentNumber.Trim(),
                DocumentDate = request.DocumentDate.HasValue
                    ? DateTime.SpecifyKind(request.DocumentDate.Value.Date, DateTimeKind.Utc)
                    : (DateTime?)null,
                Amount = request.Amount,
                RecipientName = request.RecipientName.Trim(),
                RecipientInn = string.IsNullOrWhiteSpace(request.RecipientInn) ? null : request.RecipientInn.Trim(),
                RecipientKpp = string.IsNullOrWhiteSpace(request.RecipientKpp) ? null : request.RecipientKpp.Trim(),
                RecipientAccountNumber = string.IsNullOrWhiteSpace(request.RecipientAccountNumber) ? null : request.RecipientAccountNumber.Trim(),
                RecipientBankName = string.IsNullOrWhiteSpace(request.RecipientBankName) ? null : request.RecipientBankName.Trim(),
                RecipientBankBik = string.IsNullOrWhiteSpace(request.RecipientBankBik) ? null : request.RecipientBankBik.Trim(),
                PaymentPriority = request.PaymentPriority,
                PaymentType = string.IsNullOrWhiteSpace(request.PaymentType) ? null : request.PaymentType.Trim(),
                VatType = string.IsNullOrWhiteSpace(request.VatType) ? null : request.VatType.Trim(),
                VatAmount = request.VatAmount,
                Purpose = string.IsNullOrWhiteSpace(request.Purpose) ? "Без назначения" : request.Purpose.Trim(),
                Status = "Draft",
                CreatedByUserId = actingUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _db.AddAsync(order);
            return Map(order);
        }

        public async Task<List<PaymentOrderDto>> ListByOrganizationAsync(Guid organizationId, Guid actingUserId)
        {
            if (!await _organizationService.UserIsMemberAsync(organizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var list = (await _db.GetAllAsync<PaymentOrder>())
                .Where(p => p.OrganizationId == organizationId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return list.Select(Map).ToList();
        }

        public async Task<PaymentOrderDto> SignAsync(Guid paymentOrderId, SignPaymentOrderRequest request, Guid actingUserId)
        {
            var order = await _db.GetByIdAsync<PaymentOrder>(paymentOrderId);
            if (order == null) throw new NotFoundException("Платёжное поручение не найдено");
            if (order.Status != "Draft") throw new BusinessException("Подписать можно только черновик");

            if (!await _organizationService.UserIsMemberAsync(order.OrganizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var roleOk = await _organizationService.UserIsDirectorAsync(order.OrganizationId, actingUserId)
                || (await MemberRoleAsync(order.OrganizationId, actingUserId)) == OrganizationRoles.Accountant;
            if (!roleOk) throw new BusinessException("Недостаточно прав для подписания");

            await _electronicSignatureService.EnsureDeviceAvailableAsync(request.DeviceDetected);

            
            var payload = $"{order.Id}|{order.Amount}|{order.RecipientName}|{order.RecipientAccountNumber}|{order.Purpose}";

            var org = await _db.GetByIdAsync<Organization>(order.OrganizationId);

           
            if (string.IsNullOrWhiteSpace(org?.PublicKeyPem))
                await _electronicSignatureService.EnsureDeviceAvailableAsync(request.DeviceDetected);

            await _electronicSignatureService.EnsureSignatureValidAsync(request.SignatureValue, payload, org?.PublicKeyPem);

            order.SignatureValue = request.SignatureValue.Trim();
            order.SignerCertificateThumbprint = string.IsNullOrWhiteSpace(request.CertificateThumbprint)
                ? null
                : request.CertificateThumbprint.Trim();
            order.SignedAt = DateTime.UtcNow;
            order.Status = "Signed";
            await _db.UpdateAsync(order);
            return Map(order);
        }

        public async Task<PaymentOrderDto> ExecuteAsync(Guid paymentOrderId, bool deviceDetected, Guid actingUserId)
        {
            var order = await _db.GetByIdAsync<PaymentOrder>(paymentOrderId);
            if (order == null) throw new NotFoundException("Платёжное поручение не найдено");
            if (order.Status != "Signed") throw new BusinessException("Перед исполнением поручение должно быть подписано ЭЦП");

            if (!await _organizationService.UserIsMemberAsync(order.OrganizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var roleOk = await _organizationService.UserIsDirectorAsync(order.OrganizationId, actingUserId)
                || (await MemberRoleAsync(order.OrganizationId, actingUserId)) == OrganizationRoles.Accountant;
            if (!roleOk) throw new BusinessException("Недостаточно прав для исполнения");

            await _electronicSignatureService.EnsureDeviceAvailableAsync(deviceDetected);

            if (string.IsNullOrWhiteSpace(order.RecipientAccountNumber))
                throw new BusinessException("Укажите номер счёта получателя для исполнения");

            var fromAccount = await _db.GetByIdAsync<Account>(order.FromAccountId);
            if (fromAccount == null) throw new NotFoundException("Счёт списания не найден");
            if (fromAccount.IsBlocked) throw new BusinessException("Счёт списания заблокирован");
            if (fromAccount.Balance < order.Amount) throw new BusinessException("Недостаточно средств на счёте");

            
            fromAccount.Balance -= order.Amount;
            await _db.UpdateAsync(fromAccount);

            var operationType = (await _db.GetAllAsync<OperationType>())
                .FirstOrDefault(t => t.Name == "TRANSFER" || t.Name == "Transfer" || t.Name == "transfer");
            if (operationType == null)
            {
                operationType = new OperationType { Id = Guid.NewGuid(), Name = "TRANSFER" };
                await _db.AddAsync(operationType);
            }

            var operation = new AccountOperation
            {
                Id = Guid.NewGuid(),
                FromAccountId = fromAccount.Id,
                ToAccountId = null,
                Amount = order.Amount,
                OperationTypeId = operationType.Id,
                Description = $"Платёжное поручение №{order.DocumentNumber ?? order.Id.ToString()[..8]}: {order.Purpose}. Получатель: {order.RecipientName}",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };
            await _db.AddAsync(operation);

            order.Status = "Executed";
            order.ExecutedAt = DateTime.UtcNow;
            await _db.UpdateAsync(order);
            return Map(order);
        }

        private async Task<string?> MemberRoleAsync(Guid organizationId, Guid userId)
        {
            var m = (await _db.GetAllAsync<OrganizationMember>())
                .FirstOrDefault(x => x.OrganizationId == organizationId && x.UserId == userId);
            return m?.Role;
        }

        private static PaymentOrderDto Map(PaymentOrder p) => new()
        {
            Id = p.Id,
            OrganizationId = p.OrganizationId,
            FromAccountId = p.FromAccountId,
            DocumentNumber = p.DocumentNumber,
            DocumentDate = p.DocumentDate,
            Amount = p.Amount,
            RecipientName = p.RecipientName,
            RecipientInn = p.RecipientInn,
            RecipientKpp = p.RecipientKpp,
            RecipientAccountNumber = p.RecipientAccountNumber,
            RecipientBankName = p.RecipientBankName,
            RecipientBankBik = p.RecipientBankBik,
            PaymentPriority = p.PaymentPriority,
            PaymentType = p.PaymentType,
            VatType = p.VatType,
            VatAmount = p.VatAmount,
            Purpose = p.Purpose,
            Status = p.Status,
            SignerCertificateThumbprint = p.SignerCertificateThumbprint,
            SignedAt = p.SignedAt,
            CreatedByUserId = p.CreatedByUserId,
            CreatedAt = p.CreatedAt,
            ExecutedAt = p.ExecutedAt
        };
    }
}
