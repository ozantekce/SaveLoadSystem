namespace SaveLoadSystem.Core
{
    internal interface IEncrypter
    {

        public string Encrypt(string input, string key);

        public string Decrypt(string input, string key);

        public byte[] Encrypt(byte[] input, string key);

        public byte[] Decrypt(byte[] input, string key);

    }
}

