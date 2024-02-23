using System.Collections.Generic;
namespace SaveLoadSystem.Core
{
    internal static class EncryptionHelper
    {

        private static Dictionary<EncryptionType, BaseEncrypter> Encrypters = new Dictionary<EncryptionType, BaseEncrypter>()
        {
            {EncryptionType.XOR, new XOREncrypter() },
            {EncryptionType.AES, new AESEncrypter() },
            {EncryptionType.CaesarCipher, new CaesarCipherEncrypter()},
        };


        public static byte[] Encrypt(byte[] input, EncryptionType encryptionType, string key)
        {
            if (Encrypters.ContainsKey(encryptionType))
            {
                return Encrypters[encryptionType].Encrypt(input, key);
            }
            return input;
        }

        public static byte[] Decrypt(byte[] input, EncryptionType encryptionType, string key)
        {
            if (Encrypters.ContainsKey(encryptionType))
            {
                return Encrypters[encryptionType].Decrypt(input, key);
            }
            return input;
        }


        public static string Encrypt(string input, EncryptionType encryptionType, string key)
        {
            if(Encrypters.ContainsKey(encryptionType))
            {
                return Encrypters[encryptionType].Encrypt(input, key);
            }
            return input;
        }

        public static string Decrypt(string input, EncryptionType encryptionType, string key)
        {
            if(Encrypters.ContainsKey(encryptionType))
            {
                return Encrypters[encryptionType].Decrypt(input, key);
            }
            return input;
        }


    }


}


