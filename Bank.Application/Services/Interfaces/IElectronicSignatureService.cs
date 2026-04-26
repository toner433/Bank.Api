using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface IElectronicSignatureService
    {
        Task EnsureDeviceAvailableAsync(bool deviceDetected);
        /// <summary>
        /// Проверяет RSA подпись документа с использованием публичного ключа организации.
        /// signatureValue — Base64(байты подписи)
        /// payload — строка которая была подписана
        /// publicKeyPem — PEM публичный ключ организации из БД
        /// </summary>
        Task EnsureSignatureValidAsync(string signatureValue, string payload, string? publicKeyPem);
    }
}
