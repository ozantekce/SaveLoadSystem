using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
public class SerializeSaveStrategy : ISaveLoadStrategy
{
    public string FileExtension => ".bin";

    public void Save(ISaveable saveable, string path, string fileName, bool encrypt = false, string encryptionKey = null)
    {
        fileName += FileExtension;
        path = Path.Combine(path, fileName);

        SaveableDataWrapper data = saveable.CreateSaveData();

        using (MemoryStream memoryStream = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, data);

            byte[] serializedData = memoryStream.ToArray();

            if (encrypt)
            {
                serializedData = ISaveLoadStrategy.EncryptBytes(encryptionKey, serializedData);
            }

            string serializedString = Convert.ToBase64String(serializedData);
            File.WriteAllText(path, serializedString);
        }
    }

    public SaveableDataWrapper Load(string path, string fileName, bool decrypt = false, string decryptionKey = null)
    {
        fileName += FileExtension;
        path = Path.Combine(path, fileName);
        string serializedString = File.ReadAllText(path);

        byte[] serializedData = Convert.FromBase64String(serializedString);

        if (decrypt)
        {
            serializedData = ISaveLoadStrategy.DecryptBytes(decryptionKey, serializedData);
        }

        using (MemoryStream memoryStream = new MemoryStream(serializedData))
        {
            IFormatter formatter = new BinaryFormatter();
            return (SaveableDataWrapper)formatter.Deserialize(memoryStream);
        }
    }


}
