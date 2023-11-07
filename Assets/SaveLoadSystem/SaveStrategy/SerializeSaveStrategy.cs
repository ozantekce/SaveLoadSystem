using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;



namespace SaveLoadSystem
{

    public class SerializeSaveStrategy : ISaveLoadStrategy
    {
        public string FileExtension => ".bin";

        public void Save(SaveableData saveableData, string path, string fileName, bool encrypt = false, string encryptionKey = null)
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);


            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, saveableData);

                byte[] serializedData = memoryStream.ToArray();

                if (encrypt)
                {
                    serializedData = encryptionKey.EncryptBytes(serializedData);
                }

                string serializedString = Convert.ToBase64String(serializedData);
                File.WriteAllText(path, serializedString);
            }
        }

        public SaveableData Load(string path, string fileName, bool decrypt = false, string decryptionKey = null)
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);
            string serializedString = File.ReadAllText(path);

            byte[] serializedData = Convert.FromBase64String(serializedString);

            if (decrypt)
            {
                serializedData = decryptionKey.DecryptBytes(serializedData);
            }

            using (MemoryStream memoryStream = new MemoryStream(serializedData))
            {
                IFormatter formatter = new BinaryFormatter();
                return (SaveableData)formatter.Deserialize(memoryStream);
            }
        }


    }


}
