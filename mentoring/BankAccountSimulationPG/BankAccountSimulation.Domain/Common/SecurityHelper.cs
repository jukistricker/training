namespace BankAccountSimulation.Domain.Common;
using System.Security.Cryptography;

public static class SecurityHelper
{
    public static string HashData<T>(string rawData) where T : HashAlgorithm
    {
        using var algorithm = (T)typeof(T).GetMethod("Create", Array.Empty<Type>())?.Invoke(null, null)!;

        byte[] bytes = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes);
    }
}
