using System;

namespace SaveLoadSystem.Core
{
    public class CaesarCipherEncrypter : IEncrypter
    {
        public string Encrypt(string input, string key)
        {
            int shift = CalculateShift(key);
            return EncryptImplementation(input, shift);
        }

        public string Decrypt(string input, string key)
        {
            int shift = CalculateShift(key);
            return EncryptImplementation(input, -shift);
        }

        public byte[] Encrypt(byte[] input, string key)
        {
            int shift = CalculateShift(key);
            return EncryptImplementation(input, shift);
        }

        public byte[] Decrypt(byte[] input, string key)
        {
            int shift = CalculateShift(key);
            return EncryptImplementation(input, -shift);
        }

        private string EncryptImplementation(string input, int shift)
        {
            char[] buffer = input.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (char)((buffer[i] + shift + 256) % 256);
            }
            return new string(buffer);
        }

        private byte[] EncryptImplementation(byte[] input, int shift)
        {
            byte[] result = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = (byte)((input[i] + shift + 256) % 256);
            }
            return result;
        }

        private int CalculateShift(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            int shift = 0;
            foreach (char c in key)
            {
                shift += c;
            }
            return shift % 256;
        }
    }

}

