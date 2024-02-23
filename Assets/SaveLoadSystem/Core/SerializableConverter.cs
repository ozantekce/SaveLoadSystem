using System;
using System.Collections.Generic;

using UnityEngine;

namespace SaveLoadSystem.Core
{
    internal static class SerializableConverter
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


        public static byte[] IntToBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static int BytesToInt(this byte[] value, int offset = 0)
        {
            return BitConverter.ToInt32(value, offset);
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
            byte[] bytes = new byte[12]; // 3 floats * 4 bytes per float

            Array.Copy(value.x.FloatToBytes(), 0, bytes, 0, 4);
            Array.Copy(value.y.FloatToBytes(), 0, bytes, 4, 4);
            Array.Copy(value.z.FloatToBytes(), 0, bytes, 8, 4);

            return bytes;
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
            byte[] bytes = new byte[8]; // 2 floats * 4 bytes per float

            Array.Copy(value.x.FloatToBytes(), 0, bytes, 0, 4);
            Array.Copy(value.y.FloatToBytes(), 0, bytes, 4, 4);

            return bytes;
        }


        public static Vector2 BytesToVector2(this byte[] value, int offset = 0)
        {
            return new Vector2(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4));
        }

        public static byte[] ColorToBytes(this Color value)
        {
            byte[] bytes = new byte[16]; // 4 floats * 4 bytes per float

            Array.Copy(value.r.FloatToBytes(), 0, bytes, 0, 4);
            Array.Copy(value.g.FloatToBytes(), 0, bytes, 4, 4);
            Array.Copy(value.b.FloatToBytes(), 0, bytes, 8, 4);
            Array.Copy(value.a.FloatToBytes(), 0, bytes, 12, 4);

            return bytes;
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
            byte[] bytes = new byte[16]; // 4 floats * 4 bytes per float

            Array.Copy(value.x.FloatToBytes(), 0, bytes, 0, 4);
            Array.Copy(value.y.FloatToBytes(), 0, bytes, 4, 4);
            Array.Copy(value.z.FloatToBytes(), 0, bytes, 8, 4);
            Array.Copy(value.w.FloatToBytes(), 0, bytes, 12, 4);

            return bytes;
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


        public static int BytesToInt(this byte[] value, ref int offset)
        {
            int result = BitConverter.ToInt32(value, offset);
            offset += 4; // Size of int
            return result;
        }

        public static float BytesToFloat(this byte[] value, ref int offset)
        {
            float result = BitConverter.ToSingle(value, offset);
            offset += 4; // Size of float
            return result;
        }

        public static long BytesToLong(this byte[] value, ref int offset)
        {
            long result = BitConverter.ToInt64(value, offset);
            offset += 8; // Size of long
            return result;
        }

        public static double BytesToDouble(this byte[] value, ref int offset)
        {
            double result = BitConverter.ToDouble(value, offset);
            offset += 8; // Size of double
            return result;
        }

        public static bool BytesToBool(this byte[] value, ref int offset)
        {
            bool result = BitConverter.ToBoolean(value, offset);
            offset += 1; // Size of bool
            return result;
        }

        public static Vector3 BytesToVector3(this byte[] value, ref int offset)
        {
            Vector3 result = new Vector3(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4),
                BitConverter.ToSingle(value, offset + 8));
            offset += 12; // Size of three floats
            return result;
        }

        public static Vector2 BytesToVector2(this byte[] value, ref int offset)
        {
            Vector2 result = new Vector2(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4));
            offset += 8; // Size of two floats
            return result;
        }

        public static Color BytesToColor(this byte[] value, ref int offset)
        {
            Color result = new Color(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4),
                BitConverter.ToSingle(value, offset + 8),
                BitConverter.ToSingle(value, offset + 12));
            offset += 16; // Size of four floats
            return result;
        }

        public static Quaternion BytesToQuaternion(this byte[] value, ref int offset)
        {
            Quaternion result = new Quaternion(
                BitConverter.ToSingle(value, offset),
                BitConverter.ToSingle(value, offset + 4),
                BitConverter.ToSingle(value, offset + 8),
                BitConverter.ToSingle(value, offset + 12));
            offset += 16; // Size of four floats
            return result;
        }

        public static DateTime BytesToDateTime(this byte[] value, ref int offset)
        {
            long ticks = BitConverter.ToInt64(value, offset);
            DateTime result = new DateTime(ticks);
            offset += 8; // Size of long
            return result;
        }

        public static byte[] GetVector3Bytes(this byte[] value, ref int offset)
        {
            byte[] result = new byte[12];
            Array.Copy(value, offset, result, 0, 12);
            offset += 12; // Size of three floats
            return result;
        }

        public static byte[] GetVector2Bytes(this byte[] value, ref int offset)
        {
            byte[] result = new byte[8]; // Size of two floats
            Array.Copy(value, offset, result, 0, 8);
            offset += 8;
            return result;
        }

        public static byte[] GetColorBytes(this byte[] value, ref int offset)
        {
            byte[] result = new byte[16]; // Size of four floats
            Array.Copy(value, offset, result, 0, 16);
            offset += 16;
            return result;
        }

        public static byte[] GetQuaternionBytes(this byte[] value, ref int offset)
        {
            byte[] result = new byte[16]; // Size of four floats
            Array.Copy(value, offset, result, 0, 16);
            offset += 16;
            return result;
        }

        public static byte[] GetDateTimeBytes(this byte[] value, ref int offset)
        {
            byte[] result = new byte[8]; // Size of long
            Array.Copy(value, offset, result, 0, 8);
            offset += 8;
            return result;
        }



        public static byte DataTypeToByte(this DataType dataType)
        {
            return (byte)dataType;
        }



    }

}
