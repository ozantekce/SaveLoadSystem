using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class JsonSaveStrategy : ISaveLoadStrategy
{
    public string FileExtension => ".json";

    public void Save(ISaveable saveable, string path, string fileName, bool encrypt = false, string encryptionKey = null)
    {
        fileName += FileExtension;
        path = Path.Combine(path, fileName);
        SaveableDataWrapper data = saveable.CreateSaveData();
        string jsonData = JsonConvert.SerializeObject(data);
        if (encrypt)
        {
            jsonData = ISaveLoadStrategy.EncryptString(encryptionKey, jsonData);
        }
        
        File.WriteAllText(path, jsonData);
    }

    public SaveableDataWrapper Load(string path, string fileName, bool decrypt = false, string decryptionKey = null)
    {
        fileName += FileExtension;
        path = Path.Combine(path, fileName);

        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found at " + path);
            return null;
        }

        string jsonData = File.ReadAllText(path);

        if (decrypt)
        {
            jsonData = ISaveLoadStrategy.DecryptString(decryptionKey, jsonData);
        }

        return JsonConvert.DeserializeObject<SaveableDataWrapper>(jsonData);
    }



}
