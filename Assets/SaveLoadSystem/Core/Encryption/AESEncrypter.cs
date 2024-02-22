using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncrypter : BaseEncrypter
{
    private const int KeySize = 256; // AES key size in bits
    private const int IvSize = 128;  // AES block size in bits

    public override string Encrypt(string input, string key)
    {
        byte[] iv = new byte[16]; // AES requires a 16-byte IV
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(input);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public override string Decrypt(string input, string key)
    {
        byte[] iv = new byte[16]; // AES requires a 16-byte IV
        byte[] buffer = Convert.FromBase64String(input);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }

    public override byte[] Encrypt(byte[] input, string key)
    {
        using (var aes = new AesCryptoServiceProvider())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = IvSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var keyBytes = new Rfc2898DeriveBytes(key, new byte[] { 57, 95, 245, 236, 169, 199, 51, 94, 80, 62, 64, 175, 54, 122, 232, 66 });
            aes.Key = keyBytes.GetBytes(aes.KeySize / 8);
            aes.IV = keyBytes.GetBytes(aes.BlockSize / 8);

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var bwEncrypt = new BinaryWriter(csEncrypt))
                {
                    bwEncrypt.Write(input);
                }
                return msEncrypt.ToArray();
            }
        }
    }

    public override byte[] Decrypt(byte[] input, string key)
    {
        using (var aes = new AesCryptoServiceProvider())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = IvSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var keyBytes = new Rfc2898DeriveBytes(key, new byte[] { 57, 95, 245, 236, 169, 199, 51, 94, 80, 62, 64, 175, 54, 122, 232, 66 });
            aes.Key = keyBytes.GetBytes(aes.KeySize / 8);
            aes.IV = keyBytes.GetBytes(aes.BlockSize / 8);

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var msDecrypt = new MemoryStream(input))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var brDecrypt = new BinaryReader(csDecrypt))
                {
                    byte[] decryptedData = new byte[input.Length];
                    int bytesRead = brDecrypt.Read(decryptedData, 0, decryptedData.Length);
                    Array.Resize(ref decryptedData, bytesRead);
                    return decryptedData;
                }
            }
        }
    }
}
