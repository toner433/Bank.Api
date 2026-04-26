using System;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface IPdfDocumentService
    {
        Task<byte[]> GenerateOperationReceiptPdfAsync(Guid operationId, Guid actingUserId);
        Task<byte[]> GeneratePaymentOrderPdfAsync(Guid paymentOrderId, Guid actingUserId);
    }
}

