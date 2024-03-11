using System.Collections.Generic;

namespace SaveLoadSystem.Core
{
    internal interface ISaveLoadStrategy
    {

        private static readonly Dictionary<SaveMode, ISaveLoadStrategy> Instances = new Dictionary<SaveMode, ISaveLoadStrategy>()
        {
            { SaveMode.CustomSerialize,new CustomSerializeSaveStrategy() },
            { SaveMode.Json, new JsonSaveStrategy() },
            { SaveMode.Serialize, new SerializeSaveStrategy() },
        };

        public string FileExtension { get; }

        public void Save(SavableData savableData, string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "");

        public SavableData Load(string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "");


        public static ISaveLoadStrategy GetInstance(SaveMode saveMode)
        {
            if (Instances.ContainsKey(saveMode))
            {
                return Instances[saveMode];
            }
            else
            {
                return null;
            }
        }

    }
}