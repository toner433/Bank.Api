using System.Linq;

namespace Bank.Application.Common
{
    public static class AccountNumberNormalizer
    {
        public static string NormalizeAccountNumber(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            return new string(raw.Where(c => !char.IsWhiteSpace(c) && c != '\u00a0' && c != '\u2007').ToArray());
        }

        public static string InnDigitsOnly(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            return new string(raw.Where(char.IsDigit).ToArray());
        }
    }
}
