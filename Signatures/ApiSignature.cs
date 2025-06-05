using System.Security.Cryptography;
namespace MovieApplicationApi.Signatures;

public class ApiSignature
{
    public string ComputeSignature(string secretKey, string requestBody, out string signature)
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(requestBody))
        {
            throw new ArgumentException("Secret Key, and Request Body must not be null or empty.");
        }
        using var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(requestBody));
        signature = Convert.ToBase64String(hash);
        return signature;
    }
    public bool IsValidSignature(string secretKey,string requestBody,string clientSignature)
    {

        if(string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentException("Secret Key and Signature must not be null or empty.");
        }
        using var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(requestBody));
        var computedSignature = Convert.ToBase64String(computedHash);
        return computedSignature == clientSignature;
    }
}
