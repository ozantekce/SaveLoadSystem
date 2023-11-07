using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SaveLoadSystem
{
    internal static class EncryptionExtensions
    {
        public static string EncryptString(this string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(this string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }


        private const int KeySize = 256; // AES key size in bits
        private const int IvSize = 128;  // AES block size in bits

        public static byte[] EncryptBytes(this string encryptionKey, byte[] data)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));

            using (var aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = IvSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Derive a key and IV from the encryptionKey
                var key = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }); // This is a simple salt. For better security, use a unique salt.
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var bwEncrypt = new BinaryWriter(csEncrypt))
                    {
                        bwEncrypt.Write(data);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public static byte[] DecryptBytes(this string decryptionKey, byte[] encryptedData)
        {
            if (string.IsNullOrEmpty(decryptionKey))
                throw new ArgumentNullException(nameof(decryptionKey));

            using (var aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = IvSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Derive a key and IV from the decryptionKey
                var key = new Rfc2898DeriveBytes(decryptionKey, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }); // This is a simple salt. For better security, use the same unique salt used during encryption.
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(encryptedData))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var brDecrypt = new BinaryReader(csDecrypt))
                    {
                        byte[] decryptedData = new byte[encryptedData.Length];
                        int bytesRead = brDecrypt.Read(decryptedData, 0, decryptedData.Length);
                        Array.Resize(ref decryptedData, bytesRead);
                        return decryptedData;
                    }
                }
            }
        }
    }

}
