using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SaveLoadSystem.Core
{
    [JsonObject]
    [System.Serializable]
    public class DataWrapper
    {

        [JsonProperty] private DataType t;
        [JsonProperty] private object v;

        [JsonIgnore] private byte[] bytes;
        [JsonIgnore] private List<byte[]> bytesList;


        [JsonIgnore] internal DataType Type { get => t; set => t = value; }
        [JsonIgnore] internal object Value { get => v; set => v = value; }
        [JsonIgnore] internal byte[] Bytes { get => bytes; set => bytes = value; }
        [JsonIgnore] internal List<byte[]> BytesList { get => bytesList; set => bytesList = value; }

        internal DataWrapper(object data, DataType dataType)
        {
            v = data;
            t = dataType;
        }

        internal DataWrapper(byte[] data, DataType dataType)
        {
            v = data;
            Bytes = data;
            t = dataType;
        }

        internal DataWrapper(List<byte[]> data, DataType dataType)
        {
            v = data;
            BytesList = data;
            t = dataType;
        }

        internal DataWrapper()
        {
            t = DataType.Null;
        }


        public T GetValue<T>()
        {

            if (t == DataType.Vector3)
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


        public object GetValue()
        {
            if (t == DataType.Vector3)
            {
                return SerializableConverter.ConvertToObject<Vector3>((byte[])v);
            }
            else if (t == DataType.Vector2)
            {
                return SerializableConverter.ConvertToObject<Vector2>((byte[])v);
            }
            else if (t == DataType.Color)
            {
                return SerializableConverter.ConvertToObject<Color>((byte[])v);
            }
            else if (t == DataType.Quaternion)
            {
                return SerializableConverter.ConvertToObject<Quaternion>((byte[])v);
            }
            else if (t == DataType.DateTime)
            {
                return SerializableConverter.ConvertToObject<DateTime>((byte[])v);
            }
            else if (t == DataType.List_Vector3)
            {
                return SerializableConverter.ConvertToObjectList<Vector3>((List<byte[]>)v);
            }
            else if (t == DataType.List_Vector2)
            {
                return SerializableConverter.ConvertToObjectList<Vector2>((List<byte[]>)v);
            }
            else if (t == DataType.List_Color)
            {
                return SerializableConverter.ConvertToObjectList<Color>((List<byte[]>)v);
            }
            else if (t == DataType.List_Quaternion)
            {
                return SerializableConverter.ConvertToObjectList<Quaternion>((List<byte[]>)v);
            }
            else if (t == DataType.List_DateTime)
            {
                return SerializableConverter.ConvertToObjectList<DateTime>((List<byte[]>)v);
            }

            return v;

        }


        public static DataWrapper Create<T>(T data)
        {


            if (data is ISavable savable)
            {
                return new DataWrapper(savable.ConvertToSavableData(), DataType.SavableData);
            }

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = typeof(T).GetGenericArguments()[0];
                if (typeof(ISavable).IsAssignableFrom(itemType))
                {
                    var list = data as IList;
                    var savableDataList = new List<SavableData>();
                    foreach (var item in list)
                    {
                        savableDataList.Add(((ISavable)item).ConvertToSavableData());
                    }
                    return new DataWrapper(savableDataList, DataType.List_SavableData);
                }
            }


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
                SavableData sd => new DataWrapper(sd, DataType.SavableData),
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
                List<SavableData> listSavableData => new DataWrapper(listSavableData, DataType.List_SavableData),
                null => new DataWrapper(),
                _ => throw new ArgumentException("Unsupported data type", nameof(data)),
            };


        }

    }


}


