using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace SaveLoadSystem.Core
{

    public class JsonSaveStrategy : ISaveLoadStrategy
    {
        public string FileExtension => ".json";

        public void Save(SavableData savableData, string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "")
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);
            string jsonData = JsonConvert.SerializeObject(savableData);
            if(encryptionType != EncryptionType.None)
            {
                jsonData = EncryptionHelper.Encrypt(jsonData, encryptionType, encryptionKey);
            }
            
            File.WriteAllText(path, jsonData);
        }

        public SavableData Load(string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "")
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);

            if (!File.Exists(path))
            {
                Debug.LogError("Save file not found at " + path);
                return null;
            }

            string jsonData = File.ReadAllText(path);
            if (encryptionType != EncryptionType.None)
            {
                jsonData = EncryptionHelper.Decrypt(jsonData, encryptionType, encryptionKey);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new DataWrapperConverter(), new SavableDataConverter() },
            };

            return JsonConvert.DeserializeObject<SavableData>(jsonData, settings);
        }



    }


    internal class DataWrapperConverter : JsonConverter
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
                DataType.String => new DataWrapper(data.ToObject<string>(), DataType.String),

                DataType.Vector3 => new DataWrapper(data.ToObject<byte[]>(), DataType.Vector3),
                DataType.Vector2 => new DataWrapper(data.ToObject<byte[]>(), DataType.Vector2),
                DataType.Color => new DataWrapper(data.ToObject<byte[]>(), DataType.Color),
                DataType.Quaternion => new DataWrapper(data.ToObject<byte[]>(), DataType.Quaternion),
                DataType.DateTime => new DataWrapper(data.ToObject<byte[]>(), DataType.DateTime),
                DataType.SavableData => new DataWrapper(new SavableData(data["fields"].ToObject<Dictionary<string, DataWrapper>>(serializer)), DataType.SavableData),
                // Lists
                DataType.List_Int => new DataWrapper(data.ToObject<List<int>>(serializer), DataType.List_Int),
                DataType.List_Float => new DataWrapper(data.ToObject<List<float>>(serializer), DataType.List_Float),
                DataType.List_Long => new DataWrapper(data.ToObject<List<long>>(serializer), DataType.List_Long),
                DataType.List_Double => new DataWrapper(data.ToObject<List<double>>(serializer), DataType.List_Double),
                DataType.List_Bool => new DataWrapper(data.ToObject<List<bool>>(serializer), DataType.List_Bool),
                DataType.List_String => new DataWrapper(data.ToObject<List<string>>(serializer), DataType.List_String),
                DataType.List_Vector3 => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Vector3),
                DataType.List_Vector2 => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Vector2),
                DataType.List_Color => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Color),
                DataType.List_Quaternion => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_Quaternion),
                DataType.List_DateTime => new DataWrapper(data.ToObject<List<byte[]>>(serializer), DataType.List_DateTime),
                DataType.List_SavableData => new DataWrapper(
                    data.ToObject<List<JObject>>(serializer)
                        .Select(jobj => new SavableData(jobj["fields"].ToObject<Dictionary<string, DataWrapper>>(serializer)))
                        .ToList(), DataType.List_SavableData),

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


    internal class SavableDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SavableData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var fieldToData = jsonObject["fields"].ToObject<Dictionary<string, DataWrapper>>(serializer);


            return new SavableData(fieldToData);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override bool CanWrite => false;
    }


}
