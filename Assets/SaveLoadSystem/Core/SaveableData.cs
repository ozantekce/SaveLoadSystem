using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SaveLoadSystem.Core
{

    [JsonObject]
    [System.Serializable]
    public class SaveableData
    {

        [JsonProperty] private Dictionary<string, DataWrapper> fields;

        public Dictionary<string, DataWrapper> Fields { get => fields; }

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


        public T Read<T>(string field)
        {
            return fields[field].GetValue<T>();
        }

        public object Read(string field)
        {
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
            if (fields.TryGetValue(fieldName, out DataWrapper dataWrapper))
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


    }



}