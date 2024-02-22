public abstract class BaseEncrypter
{

    public abstract string Encrypt(string input, string key);

    public abstract string Decrypt(string input, string key);

    public abstract byte[] Encrypt(byte[] input, string key);

    public abstract byte[] Decrypt(byte[] input, string key);

}