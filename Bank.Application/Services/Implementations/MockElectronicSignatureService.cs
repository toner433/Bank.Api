using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;

namespace Bank.Application.Services.Implementations
{
    public class MockElectronicSignatureService : IElectronicSignatureService
    {
        public Task EnsureDeviceAvailableAsync(bool deviceDetected)
        {
            if (!deviceDetected)
                throw new BusinessException("Устройство ЭЦП не обнаружено");
            return Task.CompletedTask;
        }

        public Task EnsureSignatureValidAsync(string signatureValue, string payload, string? publicKeyPem)
        {
            if (string.IsNullOrWhiteSpace(signatureValue))
                throw new BusinessException("Подпись ЭЦП не может быть пустой");

            // Если публичный ключ не зарегистрирован — проверяем только длину (режим совместимости)
            if (string.IsNullOrWhiteSpace(publicKeyPem))
            {
                if (signatureValue.Trim().Length < 10)
                    throw new BusinessException("Некорректная ЭЦП подпись");
                return Task.CompletedTask;
            }

            try
            {
                var sigBytes = Convert.FromBase64String(signatureValue.Trim());
                var payloadBytes = Encoding.UTF8.GetBytes(payload);

                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem.Trim());

                bool valid = rsa.VerifyData(
                    payloadBytes,
                    sigBytes,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                if (!valid)
                    throw new BusinessException("Подпись ЭЦП недействительна — данные документа не совпадают с подписью");
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (FormatException)
            {
                throw new BusinessException("Подпись ЭЦП имеет неверный формат (ожидается Base64)");
            }
            catch (CryptographicException ex)
            {
                throw new BusinessException($"Ошибка проверки ЭЦП: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
