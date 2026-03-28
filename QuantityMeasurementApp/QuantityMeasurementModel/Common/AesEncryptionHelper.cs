using System.Security.Cryptography;
using System.Text;

namespace QuantityMeasurementModel.Common
{
    /// <summary>
    /// UC17: AES-256-CBC encryption/decryption utility.
    /// Output format: Base64( IV[16 bytes] + CipherText )
    /// Key is derived via SHA-256 so any string length is accepted.
    /// </summary>
    public static class AesEncryptionHelper
    {
        public static string Encrypt(string plainText, string key)
        {
            var keyBytes = DeriveKey(key);
            using var aes = Aes.Create();
            aes.Key = keyBytes; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();
            using var enc = aes.CreateEncryptor();
            var plain  = Encoding.UTF8.GetBytes(plainText);
            var cipher = enc.TransformFinalBlock(plain, 0, plain.Length);
            var result = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText, string key)
        {
            var keyBytes  = DeriveKey(key);
            var fullBytes = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = keyBytes; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
            var iv = new byte[16]; var cipher = new byte[fullBytes.Length - 16];
            Buffer.BlockCopy(fullBytes, 0, iv, 0, 16);
            Buffer.BlockCopy(fullBytes, 16, cipher, 0, cipher.Length);
            aes.IV = iv;
            using var dec = aes.CreateDecryptor();
            return Encoding.UTF8.GetString(dec.TransformFinalBlock(cipher, 0, cipher.Length));
        }

        private static byte[] DeriveKey(string key)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }
}
