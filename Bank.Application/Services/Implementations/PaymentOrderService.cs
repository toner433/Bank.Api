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

        public PaymentOrderService(
            IDataBaseRepository db,
            IAccountRepository accountRepository,
            IOrganizationService organizationService,
            IOperationService operationService)
        {
            _db = db;
            _accountRepository = accountRepository;
            _organizationService = organizationService;
            _operationService = operationService;
        }

        public async Task<PaymentOrderDto> CreateAsync(CreatePaymentOrderRequest request, Guid actingUserId)
        {
            if (request.Amount <= 0) throw new BusinessException("Сумма должна быть больше 0");
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
                Amount = request.Amount,
                RecipientName = request.RecipientName.Trim(),
                RecipientInn = string.IsNullOrWhiteSpace(request.RecipientInn) ? null : request.RecipientInn.Trim(),
                RecipientAccountNumber = string.IsNullOrWhiteSpace(request.RecipientAccountNumber) ? null : request.RecipientAccountNumber.Trim(),
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

        public async Task<PaymentOrderDto> ExecuteAsync(Guid paymentOrderId, Guid actingUserId)
        {
            var order = await _db.GetByIdAsync<PaymentOrder>(paymentOrderId);
            if (order == null) throw new NotFoundException("Платёжное поручение не найдено");
            if (order.Status != "Draft") throw new BusinessException("Поручение уже исполнено или отменено");

            if (!await _organizationService.UserIsMemberAsync(order.OrganizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var roleOk = await _organizationService.UserIsDirectorAsync(order.OrganizationId, actingUserId)
                || (await MemberRoleAsync(order.OrganizationId, actingUserId)) == OrganizationRoles.Accountant;
            if (!roleOk) throw new BusinessException("Недостаточно прав для исполнения");

            if (string.IsNullOrWhiteSpace(order.RecipientAccountNumber))
                throw new BusinessException("Укажите номер счёта получателя для исполнения");

            await _operationService.TransferAsync(new TransferRequest
            {
                FromAccountId = order.FromAccountId,
                ToAccountNumber = order.RecipientAccountNumber,
                RecipientInn = order.RecipientInn,
                Amount = order.Amount,
                Description = $"Платёжное поручение: {order.Purpose}"
            }, actingUserId);

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
            Amount = p.Amount,
            RecipientName = p.RecipientName,
            RecipientInn = p.RecipientInn,
            RecipientAccountNumber = p.RecipientAccountNumber,
            Purpose = p.Purpose,
            Status = p.Status,
            CreatedByUserId = p.CreatedByUserId,
            CreatedAt = p.CreatedAt,
            ExecutedAt = p.ExecutedAt
        };
    }
}
