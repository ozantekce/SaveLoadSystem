using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveLoadSystem.DataWrapper;


namespace SaveLoadSystem
{
    [JsonObject]
    [System.Serializable]
    public class DataWrapper
    {

        public enum DataType
        {
            // Basic Types
            Int,
            Float,
            Long,
            Double,
            Bool,
            String,
            //

            // Others
            Vector3,
            Vector2,
            Color,
            Quaternion,
            DateTime,

            SaveableData,
            //

            // List
            List_Int,
            List_Float,
            List_Long,
            List_Double,
            List_Bool,
            List_String,

            List_Vector3,
            List_Vector2,
            List_Color,
            List_Quaternion,
            List_DateTime,

            List_SaveableData,
            //

        }


        [JsonProperty] private DataType t;
        [JsonProperty] private object v;

        [JsonIgnore] public DataType Type { get => t; set => t = value; }
        [JsonIgnore] public object Value { get => v; set => v = value; }



        public DataWrapper(object data, DataType dataType)
        {
            v = data;
            t = dataType;
        }


        public T GetValue<T>()
        {

            if(t == DataType.Vector3)
            {
                return (T)(object)SerializableConverter.ConvertToObject<Vector3>((byte[])v);
            }
            else if (t == DataType.Vector2)
            {
                return (T)(object)SerializableConverter.ConvertToObject<Vector2>((byte[])v);
            }
            else if (t == DataType.Color)
            {
                return (T)(object)SerializableConverter.ConvertToObject<Color>((byte[])v);
            }
            else if (t == DataType.Quaternion)
            {
                return (T)(object)SerializableConverter.ConvertToObject<Quaternion>((byte[])v);
            }
            else if (t == DataType.DateTime)
            {
                return (T)(object)SerializableConverter.ConvertToObject<DateTime>((byte[])v);
            }
            else if (t == DataType.List_Vector3)
            {
                return (T)(object)SerializableConverter.ConvertToObjectList<Vector3>((List<byte[]>)v);
            }
            else if (t == DataType.List_Vector2)
            {
                return (T)(object)SerializableConverter.ConvertToObjectList<Vector2>((List<byte[]>)v);
            }
            else if (t == DataType.List_Color)
            {
                return (T)(object)SerializableConverter.ConvertToObjectList<Color>((List<byte[]>)v);
            }
            else if (t == DataType.List_Quaternion)
            {
                return (T)(object)SerializableConverter.ConvertToObjectList<Quaternion>((List<byte[]>)v);
            }
            else if (t == DataType.List_DateTime)
            {
                return (T)(object)SerializableConverter.ConvertToObjectList<DateTime>((List<byte[]>)v);
            }

            return (T)v;
        }


        public static DataWrapper Create<T>(T data)
        {
            return data switch
            {
                int i => new DataWrapper(i, DataType.Int),
                float f => new DataWrapper(f, DataType.Float),
                long l => new DataWrapper(l, DataType.Long),
                double d => new DataWrapper(d, DataType.Double),
                bool b => new DataWrapper(b, DataType.Bool),
                string s => new DataWrapper(s, DataType.String),

                Vector3 v3 => new DataWrapper(v3.Vector3ToBytes(), DataType.Vector3),
                Vector2 v2 => new DataWrapper(v2.Vector2ToBytes(), DataType.Vector2),
                Color c => new DataWrapper(c.ColorToBytes(), DataType.Color),
                Quaternion q => new DataWrapper(q.QuaternionToBytes(), DataType.Quaternion),
                DateTime dt => new DataWrapper(dt.DateTimeToBytes(), DataType.DateTime),
                SaveableData sd => new DataWrapper(sd, DataType.SaveableData),
                // List
                List<int> listInt => new DataWrapper(listInt, DataType.List_Int),
                List<float> listFloat => new DataWrapper(listFloat, DataType.List_Float),
                List<long> listLong => new DataWrapper(listLong, DataType.List_Long),
                List<double> listDouble => new DataWrapper(listDouble, DataType.List_Double),
                List<bool> listBool => new DataWrapper(listBool, DataType.List_Bool),
                List<string> listString => new DataWrapper(listString, DataType.List_String),
                List<Vector3> listVector3 => new DataWrapper(listVector3.ConvertToBytesList(), DataType.List_Vector3),
                List<Vector2> listVector2 => new DataWrapper(listVector2.ConvertToBytesList(), DataType.List_Vector2),
                List<Color> listColor => new DataWrapper(listColor.ConvertToBytesList(), DataType.List_Color),
                List<Quaternion> listQuaternion => new DataWrapper(listQuaternion.ConvertToBytesList(), DataType.List_Quaternion),
                List<DateTime> listDateTime => new DataWrapper(listDateTime.ConvertToBytesList(), DataType.List_DateTime),
                List<SaveableData> listSaveableData => new DataWrapper(listSaveableData, DataType.List_SaveableData),
                _ => throw new ArgumentException("Unsupported data type", nameof(data)),
            };
        }


    }


}


