namespace SaveLoadSystem.Core
{
    public class XOREncrypter : IEncrypter
    {
        public string Encrypt(string input, string key)
        {
            return ProcessXOREncryption(input, key);
        }

        public string Decrypt(string input, string key)
        {
            return ProcessXOREncryption(input, key);
        }

        public byte[] Encrypt(byte[] input, string key)
        {
            return ProcessXOREncryption(input, key);
        }

        public byte[] Decrypt(byte[] input, string key)
        {
            return ProcessXOREncryption(input, key);
        }

        private string ProcessXOREncryption(string input, string key)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] resultBytes = ProcessXOREncryption(inputBytes, key);

            return System.Text.Encoding.UTF8.GetString(resultBytes);
        }

        private byte[] ProcessXOREncryption(byte[] input, string key)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] result = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                result[i] = (byte)(input[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return result;
        }
    }

}

