using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SaveLoadSystem.DataWrapper;

namespace SaveLoadSystem
{
    internal static class SerializableConverter
    {


        public static readonly Dictionary<DataType, byte> DataTypeToByteDict = new Dictionary<DataType, byte>
        {
            { DataType.Int, (byte)DataType.Int },
            { DataType.Float, (byte)DataType.Float },
            { DataType.Long, (byte)DataType.Long },
            { DataType.Double, (byte)DataType.Double },
            { DataType.Bool, (byte)DataType.Bool },
            { DataType.String, (byte)DataType.String },
            { DataType.Vector3, (byte)DataType.Vector3 },
            { DataType.Vector2, (byte)DataType.Vector2 },
            { DataType.Color, (byte)DataType.Color },
            { DataType.Quaternion, (byte)DataType.Quaternion },
            { DataType.DateTime, (byte)DataType.DateTime },
            { DataType.SaveableData, (byte)DataType.SaveableData },
            { DataType.List_Int, (byte)DataType.List_Int },
            { DataType.List_Float, (byte)DataType.List_Float },
            { DataType.List_Long, (byte)DataType.List_Long },
            { DataType.List_Double, (byte)DataType.List_Double },
            { DataType.List_Bool, (byte)DataType.List_Bool },
            { DataType.List_String, (byte)DataType.List_String },
            { DataType.List_Vector3, (byte)DataType.List_Vector3 },
            { DataType.List_Vector2, (byte)DataType.List_Vector2 },
            { DataType.List_Color, (byte)DataType.List_Color },
            { DataType.List_Quaternion, (byte)DataType.List_Quaternion },
            { DataType.List_DateTime, (byte)DataType.List_DateTime },
            { DataType.List_SaveableData, (byte)DataType.List_SaveableData },
        };


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



        private static bool UseCache = true;

        private static byte[] IntBytes = new byte[4];
        private static byte[] LongBytes = new byte[8];


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
            if(!UseCache) return BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                // Little-endian: least significant byte first
                IntBytes[0] = (byte)value;
                IntBytes[1] = (byte)(value >> 8);
                IntBytes[2] = (byte)(value >> 16);
                IntBytes[3] = (byte)(value >> 24);
            }
            else
            {
                // Big-endian: most significant byte first
                IntBytes[0] = (byte)(value >> 24);
                IntBytes[1] = (byte)(value >> 16);
                IntBytes[2] = (byte)(value >> 8);
                IntBytes[3] = (byte)value;
            }

            return IntBytes;
        }

        public static int BytesToInt(this byte[] value, int offset = 0)
        {
            return BitConverter.ToInt32(value, offset);
        }

        public static byte[] FloatToBytes(this float value)
        {
            if(!UseCache) return BitConverter.GetBytes(value);

            int intValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            return intValue.IntToBytes();
        }

        public static float BytesToFloat(this byte[] value, int offset = 0)
        {
            return BitConverter.ToSingle(value, offset);
        }

        public static byte[] LongToBytes(this long value)
        {
            if(!UseCache) return BitConverter.GetBytes(value);


            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < LongBytes.Length; i++)
                {
                    LongBytes[i] = (byte)(value >> (8 * i));
                }
            }
            else
            {
                for (int i = 0; i < LongBytes.Length; i++)
                {
                    LongBytes[7 - i] = (byte)(value >> (8 * i));
                }
            }

            return LongBytes;
        }

        public static long BytesToLong(this byte[] value, int offset = 0)
        {
            return BitConverter.ToInt64(value, offset);
        }

        public static byte[] DoubleToBytes(this double value)
        {
            if(!UseCache) return BitConverter.GetBytes(value);

            // Convert double to long representation
            long longValue = BitConverter.DoubleToInt64Bits(value);
            return longValue.LongToBytes(); // Reuse the LongToBytes method

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
            return DataTypeToByteDict[dataType];
        }



    }

}
