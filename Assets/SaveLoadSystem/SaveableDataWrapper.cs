using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[JsonObject]
[System.Serializable]
public class SaveableDataWrapper
{

    private delegate object DataConverter(object data);

    [JsonIgnore]
    private static readonly HashSet<Type> SupportedTypes = new HashSet<Type>
    {
        // Basic Types
        typeof(string),
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(bool),
        

        // Unity Types
        typeof(Vector3),
        typeof(Vector2),
        typeof(Color),
        typeof(Quaternion),

        // System Types
        typeof(DateTime),

        // Custom Types
        typeof(SaveableDataWrapper)

    };

    [JsonIgnore]
    private static readonly Dictionary<(Type, Type), DataConverter> ConversionStrategies = new Dictionary<(Type, Type), DataConverter>
    {
        {(typeof(double), typeof(float)), data => (float)(double)data},
        {(typeof(long), typeof(int)), data => (int)(long)data},
        {(typeof(byte[]), typeof(Vector3)), data => DeserializeBytesToVector3((byte[])data)},
        {(typeof(string), typeof(Vector3)), data => DeserializeBytesToVector3(Convert.FromBase64String((string)data))},
        {(typeof(byte[]), typeof(Color)), data => DeserializeBytesToColor((byte[])data)},
        {(typeof(string), typeof(Color)), data => DeserializeBytesToColor(Convert.FromBase64String((string)data))},
        {(typeof(byte[]), typeof(Vector2)), data => DeserializeBytesToVector2((byte[])data)},
        {(typeof(string), typeof(Vector2)), data => DeserializeBytesToVector2(Convert.FromBase64String((string)data))},
        {(typeof(byte[]), typeof(Quaternion)), data => DeserializeBytesToQuaternion((byte[])data)},
        {(typeof(string), typeof(Quaternion)), data => DeserializeBytesToQuaternion(Convert.FromBase64String((string)data))},
        {(typeof(long), typeof(System.DateTime)), data => DeserializeDateTimeFromBinary((long)data)},
    };

    [JsonIgnore]
    private static readonly Dictionary<Type, Func<object, object>> SerializableConversionStrategies = new Dictionary<Type, Func<object, object>>
    {
        { typeof(Vector3), data => SerializeVector3ToBytes((Vector3)data) },
        { typeof(Color), data => SerializeColorToBytes((Color)data) },
        { typeof(Vector2), data => SerializeVector2ToBytes((Vector2)data) },
        { typeof(Quaternion), data => SerializeQuaternionToBytes((Quaternion)data) },
        { typeof(DateTime), data => ((DateTime)data).ToBinary() }
    };



    [JsonProperty]
    private Dictionary<string, object> _fieldData = new Dictionary<string, object>();

    [JsonIgnore]
    public List<string> Fields => _fieldData.Keys.ToList();



    public void Write<T>(string field, T data)
    {
        Type dataType = typeof(T);

        if (!SupportedTypes.Contains(dataType))
        {
            throw new InvalidOperationException($"Unsupported data type {dataType.Name}.");
        }
        _fieldData[field] = ConvertToSerializableForm((T)(object)data);
    }
    public void Write<T>(string field, List<T> data)
    {
        Type dataType = typeof(T);

        if (!SupportedTypes.Contains(dataType))
        {
            throw new InvalidOperationException($"Unsupported data type {dataType.Name}.");
        }
        List<object> dataList = new List<object>();

        foreach (var item in data)
        {
            dataList.Add(ConvertToSerializableForm(item));
        }

        _fieldData[field] = dataList;
    }
    public void Write<T>(string field, T[] data)
    {
        Type dataType = typeof(T);

        if (!SupportedTypes.Contains(dataType))
        {
            throw new InvalidOperationException($"Unsupported data type {dataType.Name}.");
        }
        List<object> dataList = new List<object>();

        foreach (var item in data)
        {
            dataList.Add(ConvertToSerializableForm(item));
        }

        _fieldData[field] = dataList;
    }



    public E Read<E>(string field)
    {
        if (!_fieldData.ContainsKey(field))
            throw new KeyNotFoundException($"Field {field} not found.");

        object data = _fieldData[field];

        if (data is E eValue)
            return eValue;

        var key = (data.GetType(), typeof(E));
        if (ConversionStrategies.ContainsKey(key))
            return (E)ConversionStrategies[key](data);

        if (data is JObject jObjectData && typeof(E) == typeof(SaveableDataWrapper))
            return (E)(object)jObjectData.ToObject<SaveableDataWrapper>();

        if (data is JArray jArrayData)
        {
            if (typeof(E).IsGenericType && typeof(E).GetGenericTypeDefinition() == typeof(List<>))
                return ConvertJArrayToList<E>(jArrayData);

            if (typeof(E).IsArray)  // If E is an array type
                return ConvertJArrayToArray<E>(jArrayData);
        }

        throw new InvalidOperationException($"Expected data of type {typeof(E).Name} for field {field}, but found {data.GetType().Name}");
    }

    public bool TryRead<E>(string field, out E value)
    {
        if (!_fieldData.ContainsKey(field))
        {
            value = default(E);
            return false;
        }
        value = Read<E>(field);
        return true;
    }



    private E ConvertJArrayToList<E>(JArray jArrayData)
    {
        Type listType = typeof(E).GetGenericArguments()[0];
        var listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
        foreach (var item in jArrayData)
            ((IList)listInstance).Add(item.ToObject(listType));
        return (E)listInstance;
    }

    private E ConvertJArrayToArray<E>(JArray jArrayData)
    {
        Type elementType = typeof(E).GetElementType();  // Get the element type of the array
        var arrayInstance = Array.CreateInstance(elementType, jArrayData.Count);

        for (int i = 0; i < jArrayData.Count; i++)
        {
            var item = jArrayData[i];
            arrayInstance.SetValue(item.ToObject(elementType), i);
        }

        return (E)(object)arrayInstance;
    }


    private object ConvertToSerializableForm<T>(T data)
    {
        if (SerializableConversionStrategies.TryGetValue(typeof(T), out var converter))
        {
            return converter(data);
        }
        return data;
    }


    #region Selialize Methods
    private static byte[] SerializeVector3ToBytes(Vector3 data)
    {
        byte[] xValueBytes = BitConverter.GetBytes(data.x);
        byte[] yValueBytes = BitConverter.GetBytes(data.y);
        byte[] zValueBytes = BitConverter.GetBytes(data.z);

        byte[] result = new byte[xValueBytes.Length + yValueBytes.Length + zValueBytes.Length];
        Array.Copy(xValueBytes, 0, result, 0, xValueBytes.Length);
        Array.Copy(yValueBytes, 0, result, xValueBytes.Length, yValueBytes.Length);
        Array.Copy(zValueBytes, 0, result, xValueBytes.Length + yValueBytes.Length, zValueBytes.Length);

        return result;
    }

    private static Vector3 DeserializeBytesToVector3(byte[] dataBytes)
    {
        float x = BitConverter.ToSingle(dataBytes, 0);
        float y = BitConverter.ToSingle(dataBytes, sizeof(float));
        float z = BitConverter.ToSingle(dataBytes, sizeof(float) * 2);

        return new Vector3(x, y, z);
    }

    private static byte[] SerializeColorToBytes(Color color)
    {
        byte[] rValueBytes = BitConverter.GetBytes(color.r);
        byte[] gValueBytes = BitConverter.GetBytes(color.g);
        byte[] bValueBytes = BitConverter.GetBytes(color.b);
        byte[] aValueBytes = BitConverter.GetBytes(color.a);

        byte[] result = new byte[rValueBytes.Length + gValueBytes.Length + bValueBytes.Length + aValueBytes.Length];
        Array.Copy(rValueBytes, 0, result, 0, rValueBytes.Length);
        Array.Copy(gValueBytes, 0, result, rValueBytes.Length, gValueBytes.Length);
        Array.Copy(bValueBytes, 0, result, rValueBytes.Length + gValueBytes.Length, bValueBytes.Length);
        Array.Copy(aValueBytes, 0, result, rValueBytes.Length + gValueBytes.Length + bValueBytes.Length, aValueBytes.Length);

        return result;
    }

    private static Color DeserializeBytesToColor(byte[] dataBytes)
    {
        float r = BitConverter.ToSingle(dataBytes, 0);
        float g = BitConverter.ToSingle(dataBytes, sizeof(float));
        float b = BitConverter.ToSingle(dataBytes, sizeof(float) * 2);
        float a = BitConverter.ToSingle(dataBytes, sizeof(float) * 3);

        return new Color(r, g, b, a);
    }


    private static byte[] SerializeVector2ToBytes(Vector2 data)
    {
        byte[] xValueBytes = BitConverter.GetBytes(data.x);
        byte[] yValueBytes = BitConverter.GetBytes(data.y);

        byte[] result = new byte[xValueBytes.Length + yValueBytes.Length];
        Array.Copy(xValueBytes, 0, result, 0, xValueBytes.Length);
        Array.Copy(yValueBytes, 0, result, xValueBytes.Length, yValueBytes.Length);

        return result;
    }

    private static Vector2 DeserializeBytesToVector2(byte[] dataBytes)
    {
        float x = BitConverter.ToSingle(dataBytes, 0);
        float y = BitConverter.ToSingle(dataBytes, sizeof(float));

        return new Vector2(x, y);
    }

    private static byte[] SerializeQuaternionToBytes(Quaternion data)
    {
        byte[] xValueBytes = BitConverter.GetBytes(data.x);
        byte[] yValueBytes = BitConverter.GetBytes(data.y);
        byte[] zValueBytes = BitConverter.GetBytes(data.z);
        byte[] wValueBytes = BitConverter.GetBytes(data.w);

        byte[] result = new byte[xValueBytes.Length + yValueBytes.Length + zValueBytes.Length + wValueBytes.Length];
        Array.Copy(xValueBytes, 0, result, 0, xValueBytes.Length);
        Array.Copy(yValueBytes, 0, result, xValueBytes.Length, yValueBytes.Length);
        Array.Copy(zValueBytes, 0, result, xValueBytes.Length + yValueBytes.Length, zValueBytes.Length);
        Array.Copy(wValueBytes, 0, result, xValueBytes.Length + yValueBytes.Length + zValueBytes.Length, wValueBytes.Length);

        return result;
    }

    private static Quaternion DeserializeBytesToQuaternion(byte[] dataBytes)
    {
        float x = BitConverter.ToSingle(dataBytes, 0);
        float y = BitConverter.ToSingle(dataBytes, sizeof(float));
        float z = BitConverter.ToSingle(dataBytes, sizeof(float) * 2);
        float w = BitConverter.ToSingle(dataBytes, sizeof(float) * 3);

        return new Quaternion(x, y, z, w);
    }



    private static DateTime DeserializeDateTimeFromBinary(long binaryData)
    {
        return DateTime.FromBinary(binaryData);
    }

    #endregion

}

