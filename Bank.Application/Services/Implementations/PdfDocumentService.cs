using System;
using System.Globalization;
using System.Threading.Tasks;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Bank.Application.Services.Implementations
{
    public class PdfDocumentService : IPdfDocumentService
    {
        private readonly IDataBaseRepository _db;
        private readonly IOperationService _operationService;
        private readonly IOrganizationService _organizationService;

        public PdfDocumentService(
            IDataBaseRepository db,
            IOperationService operationService,
            IOrganizationService organizationService)
        {
            _db = db;
            _operationService = operationService;
            _organizationService = organizationService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateOperationReceiptPdfAsync(Guid operationId, Guid actingUserId)
        {
            var operationEntity = await _db.GetByIdAsync<AccountOperation>(operationId);
            if (operationEntity == null) throw new NotFoundException("Операция не найдена");

            if (!await UserMayAccessOperationAsync(operationEntity, actingUserId))
                throw new BusinessException("Нет доступа к операции");

            var operation = await _operationService.GetOperationByIdAsync(operationId);
            if (operation == null) throw new NotFoundException("Операция не найдена");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Text("Квитанция по операции").SemiBold().FontSize(20);
                    page.Content().Column(col =>
                    {
                        col.Spacing(8);
                        col.Item().Text($"Номер операции: {operation.Id}");
                        col.Item().Text($"Дата: {operation.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}");
                        col.Item().Text($"Тип: {operation.OperationType}");
                        col.Item().Text($"Сумма: {operation.Amount.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"))}");
                        col.Item().Text($"Статус: {operation.Status}");
                        col.Item().Text($"Счёт списания: {operation.FromAccountNumber}");
                        col.Item().Text($"Счёт зачисления: {operation.ToAccountNumber}");
                        col.Item().Text($"Описание: {operation.Description}");
                    });
                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span($"{DateTime.Now:dd.MM.yyyy HH:mm}");
                    });
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> GeneratePaymentOrderPdfAsync(Guid paymentOrderId, Guid actingUserId)
        {
            var order = await _db.GetByIdAsync<PaymentOrder>(paymentOrderId);
            if (order == null) throw new NotFoundException("Платёжное поручение не найдено");

            if (!await _organizationService.UserIsMemberAsync(order.OrganizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Text("Платёжное поручение").SemiBold().FontSize(20);
                    page.Content().Column(col =>
                    {
                        col.Spacing(8);
                        col.Item().Text($"ID документа: {order.Id}");
                        col.Item().Text($"Номер документа: {order.DocumentNumber ?? "б/н"}");
                        col.Item().Text($"Дата документа: {(order.DocumentDate.HasValue ? order.DocumentDate.Value.ToLocalTime().ToString("dd.MM.yyyy") : "-")}");
                        col.Item().Text($"Счёт списания: {order.FromAccountId}");
                        col.Item().Text($"Получатель: {order.RecipientName}");
                        col.Item().Text($"ИНН/КПП: {order.RecipientInn ?? "-"} / {order.RecipientKpp ?? "-"}");
                        col.Item().Text($"Счёт получателя: {order.RecipientAccountNumber ?? "-"}");
                        col.Item().Text($"Банк получателя: {order.RecipientBankName ?? "-"}");
                        col.Item().Text($"БИК банка: {order.RecipientBankBik ?? "-"}");
                        col.Item().Text($"Очередность / вид платежа: {(order.PaymentPriority?.ToString() ?? "-")} / {order.PaymentType ?? "-"}");
                        col.Item().Text($"НДС: {order.VatType ?? "-"}, сумма: {(order.VatAmount?.ToString("N2", CultureInfo.GetCultureInfo("ru-RU")) ?? "-")}");
                        col.Item().Text($"Сумма: {order.Amount.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"))}");
                        col.Item().Text($"Назначение: {order.Purpose}");
                        col.Item().Text($"Статус: {order.Status}");
                    });
                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span($"{DateTime.Now:dd.MM.yyyy HH:mm}");
                    });
                });
            }).GeneratePdf();
        }

        private async Task<bool> UserMayAccessOperationAsync(AccountOperation operation, Guid userId)
        {
            if (operation.FromAccountId.HasValue)
            {
                var from = await _db.GetByIdAsync<Account>(operation.FromAccountId.Value);
                if (from != null && await UserMayAccessAccountAsync(from, userId))
                    return true;
            }

            if (operation.ToAccountId.HasValue)
            {
                var to = await _db.GetByIdAsync<Account>(operation.ToAccountId.Value);
                if (to != null && await UserMayAccessAccountAsync(to, userId))
                    return true;
            }

            return false;
        }

        private async Task<bool> UserMayAccessAccountAsync(Account account, Guid userId)
        {
            if (account.UserId.HasValue && account.UserId.Value == userId) return true;
            if (account.OrganizationId.HasValue)
                return await _organizationService.UserIsMemberAsync(account.OrganizationId.Value, userId);
            return false;
        }
    }
}

