using System;

public class CaesarCipherEncrypter : BaseEncrypter
{
    public override string Encrypt(string input, string key)
    {
        return EncryptImplementation(input, StringToInt(key));
    }

    public override string Decrypt(string input, string key)
    {
        int shift = StringToInt(key) % 26;
        return EncryptImplementation(input, 26 - shift);
    }

    public override byte[] Encrypt(byte[] input, string key)
    {
        return EncryptImplementation(input, StringToInt(key));
    }

    public override byte[] Decrypt(byte[] input, string key)
    {
        int shift = StringToInt(key) % 26;
        return EncryptImplementation(input, 26 - shift);
    }


    private string EncryptImplementation(string input, int key)
    {
        char[] buffer = input.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char letter = buffer[i];
            if (char.IsLetter(letter))
            {
                char letterOffset = char.IsUpper(letter) ? 'A' : 'a';
                letter = (char)((((letter + key) - letterOffset) % 26) + letterOffset);
            }
            buffer[i] = letter;
        }
        return new string(buffer);
    }


    private byte[] EncryptImplementation(byte[] input, int key)
    {
        byte[] result = new byte[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            byte letter = input[i];
            if (letter >= 'A' && letter <= 'Z' || letter >= 'a' && letter <= 'z')
            {
                byte letterOffset = (byte)(char.IsUpper((char)letter) ? 'A' : 'a');
                letter = (byte)((((letter + key) - letterOffset) % 26) + letterOffset);
            }
            result[i] = letter;
        }
        return result;
    }


    private int StringToInt(string key)
    {
        int hash = 7;
        for (int i = 0; i < key.Length; i++)
        {
            hash = hash * 31 + key[i];
        }
        return Math.Abs(hash % 26);
    }



}
