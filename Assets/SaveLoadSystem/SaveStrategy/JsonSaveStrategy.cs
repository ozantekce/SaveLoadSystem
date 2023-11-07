using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using static SaveLoadSystem.DataWrapper;
using System.Linq;

namespace SaveLoadSystem
{

    public class JsonSaveStrategy : ISaveLoadStrategy
    {
        public string FileExtension => ".json";

        public void Save(ISaveable saveable, string path, string fileName, bool encrypt = false, string encryptionKey = null)
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);
            SaveableData data = saveable.CreateSaveData();
            string jsonData = JsonConvert.SerializeObject(data);
            if (encrypt)
            {
                jsonData = encryptionKey.EncryptString(jsonData);
            }

            File.WriteAllText(path, jsonData);
        }

        public SaveableData Load(string path, string fileName, bool decrypt = false, string decryptionKey = null)
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
                jsonData = decryptionKey.DecryptString(jsonData);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new DataWrapperConverter(), new SaveableDataConverter() },
            };

            return JsonConvert.DeserializeObject<SaveableData>(jsonData, settings);
        }



    }


    public class DataWrapperConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DataWrapper);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            DataType dataType = item["t"].ToObject<DataType>();
            JToken data = item["v"];


            DataWrapper wrapper = dataType switch
            {
                DataType.Int => new DataWrapper(data.ToObject<int>(), DataType.Int),
                DataType.Float => new DataWrapper(data.ToObject<float>(), DataType.Float),
                DataType.Long => new DataWrapper(data.ToObject<long>(), DataType.Long),
                DataType.Double => new DataWrapper(data.ToObject<double>(), DataType.Double),
                DataType.Bool => new DataWrapper(data.ToObject<bool>(), DataType.Bool),
                DataType.Char => new DataWrapper(data.ToObject<char>(), DataType.Char),
                DataType.String => new DataWrapper(data.ToObject<string>(), DataType.String),

                DataType.Vector3 => new DataWrapper(data.ToObject<byte[]>(), DataType.Vector3),
                DataType.Vector2 => new DataWrapper(data.ToObject<byte[]>(), DataType.Vector2),
                DataType.Color => new DataWrapper(data.ToObject<byte[]>(), DataType.Color),
                DataType.Quaternion => new DataWrapper(data.ToObject<byte[]>(), DataType.Quaternion),
                DataType.DateTime => new DataWrapper(data.ToObject<byte[]>(), DataType.DateTime),
                DataType.SaveableData => new DataWrapper(new SaveableData(data["fields"].ToObject<Dictionary<string, DataWrapper>>(serializer)), DataType.SaveableData),
                // Lists
                DataType.List_Int => new DataWrapper(data.ToObject<List<int>>(serializer), DataType.List_Int),
                DataType.List_Float => new DataWrapper(data.ToObject<List<float>>(serializer), DataType.List_Float),
                DataType.List_Long => new DataWrapper(data.ToObject<List<long>>(serializer), DataType.List_Long),
                DataType.List_Double => new DataWrapper(data.ToObject<List<double>>(serializer), DataType.List_Double),
                DataType.List_Bool => new DataWrapper(data.ToObject<List<bool>>(serializer), DataType.List_Bool),
                DataType.List_Char => new DataWrapper(data.ToObject<List<char>>(serializer), DataType.List_Char),
                DataType.List_String => new DataWrapper(data.ToObject<List<string>>(serializer), DataType.List_String),
                DataType.List_Vector3 => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Vector3),
                DataType.List_Vector2 => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Vector2),
                DataType.List_Color => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Color),
                DataType.List_Quaternion => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Quaternion),
                DataType.List_DateTime => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_DateTime),
                DataType.List_SaveableData => new DataWrapper(
                    data.ToObject<List<JObject>>(serializer)
                        .Select(jobj => new SaveableData(jobj["fields"].ToObject<Dictionary<string, DataWrapper>>(serializer)))
                        .ToList(), DataType.List_SaveableData),

                _ => throw new JsonSerializationException($"Unknown DataType: {dataType}")
            };


            return wrapper;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override bool CanWrite => false;
    }


    public class SaveableDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SaveableData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var fieldToData = jsonObject["fields"].ToObject<Dictionary<string, DataWrapper>>(serializer);


            return new SaveableData(fieldToData);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Serialize the SaveableData object here
            SaveableData saveableData = (SaveableData)value;

            // Use serializer.Serialize to write the JSON for the object, if needed.
            // This is where you would handle the recursive structure manually if necessary.
            throw new NotImplementedException();
        }

        public override bool CanWrite => true; // Set to true if you implement WriteJson
    }


}
