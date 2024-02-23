using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaveLoadSystem.Core
{
    internal class SerializeSaveStrategy : ISaveLoadStrategy
    {
        public string FileExtension => ".bin";

        public void Save(SaveableData saveableData, string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "")
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, saveableData);

                byte[] serializedData = memoryStream.ToArray();
                string serializedString = Convert.ToBase64String(serializedData);
                if (encryptionType != EncryptionType.None)
                {
                    serializedString = EncryptionHelper.Encrypt(serializedString, encryptionType, encryptionKey);
                }
                File.WriteAllText(path, serializedString);
            }
        }

        public SaveableData Load(string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "")
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);

            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogError("Save file not found at " + path);
                return null;
            }

            string serializedString = File.ReadAllText(path);
            if (encryptionType != EncryptionType.None)
            {
                serializedString = EncryptionHelper.Decrypt(serializedString, encryptionType, encryptionKey);
            }

            byte[] serializedData = Convert.FromBase64String(serializedString);


            using (MemoryStream memoryStream = new MemoryStream(serializedData))
            {
                IFormatter formatter = new BinaryFormatter();
                return (SaveableData)formatter.Deserialize(memoryStream);
            }
        }


    }


}
