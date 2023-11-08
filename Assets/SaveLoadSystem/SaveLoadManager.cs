using System;
using UnityEngine;


namespace SaveLoadSystem
{
    public static class SaveLoadManager
    {

        private const string EncryptionKey = "5ZaX8nC2pY7kF4rO9gE0bL3tU1mQ6sWv";


        private static ISaveLoadStrategy SaveLoadStrategy;


        public static void Save(SaveableData saveableData, string fileName, SaveMode saveStrategy, bool encrypt = false)
        {
            if (saveStrategy == SaveMode.Json)
            {
                SaveLoadStrategy = new JsonSaveStrategy();
            }
            else if (saveStrategy == SaveMode.Serialize)
            {
                SaveLoadStrategy = new SerializeSaveStrategy();
            }
            else if (saveStrategy == SaveMode.CustomSerialize)
            {
                SaveLoadStrategy = new CustomSerializeSaveStrategy();
            }
            else
            {
                throw new NotImplementedException();
            }
            string path = Application.persistentDataPath;
            SaveLoadStrategy.Save(saveableData, path, fileName, encrypt, EncryptionKey);
        }



        public static void Save(ISaveable saveable, string fileName, SaveMode saveStrategy, bool encrypt = false)
        {
            Save(saveable.CreateSaveData(), fileName, saveStrategy, encrypt);
        }


        public static SaveableData Load(string fileName, SaveMode saveStrategy, bool encrypt = false)
        {
            // Select strategy
            if (saveStrategy == SaveMode.Json)
            {
                SaveLoadStrategy = new JsonSaveStrategy();
            }
            else if (saveStrategy == SaveMode.Serialize)
            {
                SaveLoadStrategy = new SerializeSaveStrategy();
            }
            else if (saveStrategy == SaveMode.CustomSerialize)
            {
                SaveLoadStrategy = new CustomSerializeSaveStrategy();
            }
            else
            {
                throw new NotImplementedException();
            }

            string path = Application.persistentDataPath;
            return SaveLoadStrategy.Load(path, fileName, encrypt, EncryptionKey);
        }




    }

    public enum SaveMode
    {
        Json,
        Serialize,
        CustomSerialize
    }


}
