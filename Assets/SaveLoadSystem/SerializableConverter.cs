using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace SaveLoadSystem
{
    public static class SerializableConverter
    {


        private static readonly Dictionary<Type, Func<object, byte[]>> SerializableConversionStrategies = new Dictionary<Type, Func<object, byte[]>>
        {
            { typeof(Vector3), data => Vector3ToBytes((Vector3)data) },
            { typeof(Color), data => ColorToBytes((Color)data) },
            { typeof(Vector2), data => Vector2ToBytes((Vector2)data) },
            { typeof(Quaternion), data => QuaternionToBytes((Quaternion)data) },
            { typeof(DateTime), data => DateTimeToBytes((DateTime)data) },
        };

        private static readonly Dictionary<Type, Func<byte[], object>> DeserializableConversionStrategies = new Dictionary<Type, Func<byte[], object>>
        {
            { typeof(Vector3),data => BytesToVector3(data) },
            { typeof(Color),data => BytesToColor(data) },
            { typeof(Vector2),data => BytesToVector2(data) },
            { typeof(Quaternion),data  => BytesToQuaternion(data) },
            { typeof(DateTime),data => BytesToDateTime(data) },
        };


        public static byte[] ConvertToBytes<T>(this T data)
        {
            if (SerializableConversionStrategies.TryGetValue(typeof(T), out var converter))
            {
                return converter(data);
            }
            return default;
        }

        public static T ConvertToObject<T>(this byte[] data)
        {
            if (DeserializableConversionStrategies.TryGetValue(typeof(T), out var converter))
            {
                return (T)converter(data);
            }
            return default;
        }


        public static List<byte[]> ConvertToBytesList<T>(this List<T> data)
        {
            List<byte[]> converted = new List<byte[]>();
            foreach (T t in data)
            {
                converted.Add(ConvertToBytes(t));
            }

            return converted;
        }

        public static List<T> ConvertToObjectList<T>(this List<byte[]> bytesList)
        {
            List<T> converted = new List<T>();
            foreach (byte[] bytes in bytesList)
            {
                converted.Add(ConvertToObject<T>(bytes));
            }

            return converted;
        }



        public static byte[] FloatToBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static float BytesToFloat(this byte[] value, int offset = 0)
        {
            return BitConverter.ToSingle(value, offset);
        }

        public static byte[] LongToBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static long BytesToLong(this byte[] value, int offset = 0)
        {
            return BitConverter.ToInt64(value, offset);
        }

        public static byte[] DoubleToBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }

        public static double BytesToDouble(this byte[] value, int offset = 0)
        {
            return BitConverter.ToDouble(value, offset);
        }

        public static byte[] BoolToBytes(this bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public static bool BytesToBool(this byte[] value, int offset = 0)
        {
            return BitConverter.ToBoolean(value, offset);
        }

        public static byte[] Vector3ToBytes(this Vector3 value)
        {
            return BitConverter.GetBytes(value.x)
                .Concat(BitConverter.GetBytes(value.y))
                .Concat(BitConverter.GetBytes(value.z))
                .ToArray();
        }

        public static Vector3 BytesToVector3(this byte[] value, int offset = 0)
        {
            return new Vector3(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4),
                BitConverter.ToSingle(value, offset + 8));
        }

        public static byte[] Vector2ToBytes(this Vector2 value)
        {
            return BitConverter.GetBytes(value.x)
                .Concat(BitConverter.GetBytes(value.y))
                .ToArray();
        }

        public static Vector2 BytesToVector2(this byte[] value, int offset = 0)
        {
            return new Vector2(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4));
        }

        public static byte[] ColorToBytes(this Color value)
        {
            return BitConverter.GetBytes(value.r)
                .Concat(BitConverter.GetBytes(value.g))
                .Concat(BitConverter.GetBytes(value.b))
                .Concat(BitConverter.GetBytes(value.a))
                .ToArray();
        }

        public static Color BytesToColor(this byte[] value, int offset = 0)
        {
            return new Color(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4),
                BitConverter.ToSingle(value, offset + 8),
                BitConverter.ToSingle(value, offset + 12));
        }

        public static byte[] QuaternionToBytes(this Quaternion value)
        {
            return BitConverter.GetBytes(value.x)
                .Concat(BitConverter.GetBytes(value.y))
                .Concat(BitConverter.GetBytes(value.z))
                .Concat(BitConverter.GetBytes(value.w))
                .ToArray();
        }

        public static Quaternion BytesToQuaternion(this byte[] value, int offset = 0)
        {
            return new Quaternion(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4),
                BitConverter.ToSingle(value, offset + 8),
                BitConverter.ToSingle(value, offset + 12));
        }

        public static byte[] DateTimeToBytes(this DateTime value)
        {
            long ticks = value.Ticks;
            return BitConverter.GetBytes(ticks);
        }

        public static DateTime BytesToDateTime(this byte[] value, int offset = 0)
        {
            long ticks = BitConverter.ToInt64(value, offset);
            return new DateTime(ticks);
        }
    }

}
