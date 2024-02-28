using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem.Core
{
    [JsonObject]
    [System.Serializable]
    public class SaveableData
    {

        [JsonProperty] private Dictionary<string, DataWrapper> fields;
        [JsonIgnore] public Dictionary<string, DataWrapper> Fields { get => fields; }

        public SaveableData()
        {
            fields = new Dictionary<string, DataWrapper>();
        }

        internal SaveableData(Dictionary<string, DataWrapper> fieldToData)
        {
            fields = fieldToData;
        }

        public void Write<T>(string field, T value)
        {
            if (fields.ContainsKey(field))
            {
                throw new ArgumentException($"A field with the name '{field}' already exists. Duplicate fields are not allowed.");
            }
            fields[field] = DataWrapper.Create(value);
        }


        public void Update<T>(string field, T value)
        {
            fields[field] = DataWrapper.Create(value);
        }

        public T Read<T>(string field)
        {
            if (!fields.ContainsKey(field))
            {
                Debug.LogError($"A field with the name '{field}' not exists. Returning default value: '{default(T)}'");
                return default;
            }

            return fields[field].GetValue<T>();
        }

        public object Read(string field)
        {
            if (!fields.ContainsKey(field))
            {
                Debug.LogError($"A field with the name '{field}' not exists. Returning default value");
                return default;
            }
            return fields[field].GetValue();
        }

        public bool TryRead<T>(string field, out T data)
        {
            if (fields.ContainsKey(field))
            {
                data = Read<T>(field);
                return true;
            }
            else
            {
                data = default;
                return false;
            }
        }

        public bool TryRead(string fieldName, out object loadedValue)
        {
            if (fields.ContainsKey(fieldName))
            {
                loadedValue = Read(fieldName);
                return true;
            }
            else
            {
                loadedValue = null;
                return false;
            }
        }

        public T ReadOrDefault<T>(string field, T defaultValue)
        {
            if (!fields.ContainsKey(field))
            {
                return defaultValue;
            }
            else
            {
                return fields[field].GetValue<T>();
            }
        }

        public T ReadOrDefault<T>(string field)
        {
            if (!fields.ContainsKey(field))
            {
                return default;
            }
            else
            {
                return fields[field].GetValue<T>();
            }
        }

    }

}