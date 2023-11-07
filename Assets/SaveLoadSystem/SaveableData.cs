using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SaveLoadSystem
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

        public SaveableData(Dictionary<string, DataWrapper> fieldToData)
        {
            fields = fieldToData;
        }


        public void Write<T>(string field, T value)
        {
            if (fields.ContainsKey(field))
            {
                throw new Exception("Field using");
            }
            fields[field] = DataWrapper.Create(value);

        }


        public T Read<T>(string field)
        {
            return fields[field].GetValue<T>();
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


    }



}