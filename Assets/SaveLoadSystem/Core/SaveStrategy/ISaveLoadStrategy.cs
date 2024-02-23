using System.Collections.Generic;

namespace SaveLoadSystem.Core
{
    internal interface ISaveLoadStrategy
    {

        private static Dictionary<SaveMode, ISaveLoadStrategy> Instances = new Dictionary<SaveMode, ISaveLoadStrategy>()
        {
            { SaveMode.CustomSerialize,new CustomSerializeSaveStrategy() },
            { SaveMode.Json, new JsonSaveStrategy() },
            { SaveMode.Serialize, new SerializeSaveStrategy() },
        };

        public string FileExtension { get; }

        public void Save(SaveableData saveableData, string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "");

        public SaveableData Load(string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "");


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