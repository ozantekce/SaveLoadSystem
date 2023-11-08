using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static SaveLoadSystem.DataWrapper;

public static class RandomValueGenerator
{
    private static readonly System.Random random = new System.Random();

    private const int StartYear = 2000;
    private const int YearRange = 200;


    /*
    public static T CreateRandom<T>(DataType type)
    {
        object randomValue = CreateRandom(type);
        return (T)randomValue;
    }*/

    public static object CreateRandom(DataType type)
    {
        switch (type)
        {
            case DataType.Int:
                return RandomValueGenerator.CreateRandomInt();
            case DataType.Float:
                return RandomValueGenerator.CreateRandomFloat();
            case DataType.Long:
                return RandomValueGenerator.CreateRandomLong();
            case DataType.Double:
                return RandomValueGenerator.CreateRandomDouble();
            case DataType.Bool:
                return RandomValueGenerator.CreateRandomBool();
            case DataType.String:
                return RandomValueGenerator.CreateRandomString();
            case DataType.Vector3:
                return RandomValueGenerator.CreateRandomVector3();
            case DataType.Vector2:
                return RandomValueGenerator.CreateRandomVector2();
            case DataType.Color:
                return RandomValueGenerator.CreateRandomColor();
            case DataType.Quaternion:
                return RandomValueGenerator.CreateRandomQuaternion();
            case DataType.DateTime:
                return RandomValueGenerator.CreateRandomDateTime();
            case DataType.List_Int:
                return RandomValueGenerator.CreateRandomIntList();
            case DataType.List_Float:
                return RandomValueGenerator.CreateRandomFloatList();
            case DataType.List_Long:
                return RandomValueGenerator.CreateRandomLongList();
            case DataType.List_Double:
                return RandomValueGenerator.CreateRandomDoubleList();
            case DataType.List_Bool:
                return RandomValueGenerator.CreateRandomBoolList();
            case DataType.List_String:
                return RandomValueGenerator.CreateRandomStringList();
            case DataType.List_Vector3:
                return RandomValueGenerator.CreateRandomVector3List();
            case DataType.List_Vector2:
                return RandomValueGenerator.CreateRandomVector2List();
            case DataType.List_Color:
                return RandomValueGenerator.CreateRandomColorList();
            case DataType.List_Quaternion:
                return RandomValueGenerator.CreateRandomQuaternionList();
            case DataType.List_DateTime:
                return RandomValueGenerator.CreateRandomDateTimeList();
            default:
                throw new ArgumentOutOfRangeException(nameof(type), $"Not expected data type value: {type}");
        }
    }


    public static int CreateRandomInt()
    {
        return random.Next();
    }

    public static float CreateRandomFloat()
    {
        return (float)random.NextDouble();
    }

    public static long CreateRandomLong()
    {
        byte[] buffer = new byte[8];
        random.NextBytes(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }

    public static double CreateRandomDouble()
    {
        return random.NextDouble();
    }

    public static bool CreateRandomBool()
    {
        return random.Next(2) == 1;
    }

    public static char CreateRandomChar()
    {
        return (char)random.Next(char.MinValue, char.MaxValue);
    }

    public static string CreateRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, random.Next(5, 70))
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static Vector3 CreateRandomVector3()
    {
        return new Vector3(CreateRandomFloat(), CreateRandomFloat(), CreateRandomFloat());
    }

    public static Vector2 CreateRandomVector2()
    {
        return new Vector2(CreateRandomFloat(), CreateRandomFloat());
    }

    public static Color CreateRandomColor()
    {
        return new Color(CreateRandomFloat(), CreateRandomFloat(), CreateRandomFloat(), CreateRandomFloat());
    }

    public static Quaternion CreateRandomQuaternion()
    {
        return new Quaternion(CreateRandomFloat(), CreateRandomFloat(), CreateRandomFloat(), CreateRandomFloat());
    }

    public static DateTime CreateRandomDateTime()
    {
        return new DateTime(StartYear + random.Next(YearRange), random.Next(1, 13), random.Next(1, 29));
    }


    public static List<int> CreateRandomIntList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomInt()).ToList();
    }

    public static List<float> CreateRandomFloatList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomFloat()).ToList();
    }

    public static List<long> CreateRandomLongList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomLong()).ToList();
    }

    public static List<double> CreateRandomDoubleList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomDouble()).ToList();
    }

    public static List<bool> CreateRandomBoolList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomBool()).ToList();
    }

    public static List<char> CreateRandomCharList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomChar()).ToList();
    }

    public static List<string> CreateRandomStringList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomString()).ToList();
    }

    public static List<Vector3> CreateRandomVector3List()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomVector3()).ToList();
    }

    public static List<Vector2> CreateRandomVector2List()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomVector2()).ToList();
    }

    public static List<Color> CreateRandomColorList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomColor()).ToList();
    }

    public static List<Quaternion> CreateRandomQuaternionList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomQuaternion()).ToList();
    }

    public static List<DateTime> CreateRandomDateTimeList()
    {
        return Enumerable.Range(0, random.Next(1, 30)).Select(_ => CreateRandomDateTime()).ToList();
    }



}
