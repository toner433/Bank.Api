using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Application.DTOs.PaymentOrders;

namespace Bank.Application.Services.Interfaces
{
    public interface IPaymentOrderService
    {
        Task<PaymentOrderDto> CreateAsync(CreatePaymentOrderRequest request, Guid actingUserId);
        Task<List<PaymentOrderDto>> ListByOrganizationAsync(Guid organizationId, Guid actingUserId);
        Task<PaymentOrderDto> SignAsync(Guid paymentOrderId, SignPaymentOrderRequest request, Guid actingUserId);
        Task<PaymentOrderDto> ExecuteAsync(Guid paymentOrderId, bool deviceDetected, Guid actingUserId);
    }
}
