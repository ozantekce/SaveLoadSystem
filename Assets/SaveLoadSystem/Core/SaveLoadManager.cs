using System.Collections.Generic;
using UnityEngine;


namespace SaveLoadSystem.Core
{
    public static class SaveLoadManager
    {

        private const string EncryptionKey = "5ZaX8nC2pY7kF4rO9gE0bL3tU1mQ6sWv";

        private static Dictionary<SaveMode, ISaveLoadStrategy> SaveLoadStrategys = new Dictionary<SaveMode, ISaveLoadStrategy>()
        {
            {SaveMode.CustomSerialize, new CustomSerializeSaveStrategy()},
            {SaveMode.Json, new JsonSaveStrategy()},
            {SaveMode.Serialize, new SerializeSaveStrategy()},
        };

        public static void Save(SaveableData saveableData, string fileName, SaveMode saveStrategy, EncryptionType encryptionType = EncryptionType.None)
        {
            string path = Application.persistentDataPath;
            SaveLoadStrategys[saveStrategy].Save(saveableData, path, fileName, encryptionType, EncryptionKey);
        }

        public static void Save(ISaveable saveable, string fileName, SaveMode saveStrategy = SaveMode.CustomSerialize, EncryptionType encryptionType = EncryptionType.None)
        {
            Save(saveable.ConvertToSaveableData(), fileName, saveStrategy, encryptionType);
        }


        public static SaveableData Load(string fileName, SaveMode saveStrategy = SaveMode.CustomSerialize, EncryptionType encryptionType = EncryptionType.None)
        {

            string path = Application.persistentDataPath;
            return SaveLoadStrategys[saveStrategy].Load(path, fileName, encryptionType, EncryptionKey);
        }


    }




}
