using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static SaveLoadSystem.DataWrapper;

namespace SaveLoadSystem
{
    public class CustomSerializeSaveStrategy : ISaveLoadStrategy
    {
        public string FileExtension => ".cus";

        public void Save(SaveableData saveableData, string path, string fileName, bool encrypt = false, string encryptionKey = null)
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);

            byte[] serializedData = Serialize(saveableData);

            if (encrypt)
            {
                serializedData = encryptionKey.EncryptBytes(serializedData);
            }
            File.WriteAllBytes(path, serializedData);
        }

        public SaveableData Load(string path, string fileName, bool decrypt = false, string decryptionKey = null)
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);

            byte[] serializedData = File.ReadAllBytes(path);

            if (decrypt)
            {
                serializedData = decryptionKey.DecryptBytes(serializedData);
            }

            return Deserialize(serializedData);
        }



        public static byte[] Serialize(SaveableData saveableData)
        {
            List<byte> serializedData = new List<byte>();
            Dictionary<string, DataWrapper> fields = saveableData.Fields;

            serializedData.AddRange(BitConverter.GetBytes(fields.Count));

            foreach (var item in fields)
            {
                byte[] fieldNameBytes = System.Text.Encoding.UTF8.GetBytes(item.Key);
                serializedData.AddRange(BitConverter.GetBytes(fieldNameBytes.Length));
                serializedData.AddRange(fieldNameBytes);

                serializedData.AddRange(ConvertToByte(item.Value));
            }

            return serializedData.ToArray();
        }

        private static byte[] ConvertToByte(DataWrapper data)
        {

            switch (data.Type)
            {
                case DataType.Int:
                    return new byte[] { (byte)DataType.Int }.Concat(data.GetValue<int>().IntToBytes()).ToArray();
                case DataType.String:
                    return new byte[] { (byte)DataType.String }.Concat(BitConverter.GetBytes(data.GetValue<string>().Length)).Concat(System.Text.Encoding.UTF8.GetBytes(data.GetValue<string>())).ToArray();
                case DataType.Float:
                    return new byte[] { (byte)DataType.Float }.Concat(data.GetValue<float>().FloatToBytes()).ToArray();
                case DataType.Long:
                    return new byte[] { (byte)DataType.Long }.Concat(data.GetValue<long>().LongToBytes()).ToArray();
                case DataType.Double:
                    return new byte[] { (byte)DataType.Double }.Concat(data.GetValue<double>().DoubleToBytes()).ToArray();
                case DataType.Bool:
                    return new byte[] { (byte)DataType.Bool, Convert.ToByte(data.GetValue<bool>()) };
                case DataType.Vector3:
                    return new byte[] { (byte)DataType.Vector3 }.Concat((byte[])data.Value).ToArray();
                case DataType.Vector2:
                    return new byte[] { (byte)DataType.Vector2 }.Concat((byte[])data.Value).ToArray();
                case DataType.Color:
                    return new byte[] { (byte)DataType.Color }.Concat((byte[])data.Value).ToArray();
                case DataType.Quaternion:
                    return new byte[] { (byte)DataType.Quaternion }.Concat((byte[])data.Value).ToArray();
                case DataType.DateTime:
                    return new byte[] { (byte)DataType.DateTime }.Concat((byte[])data.Value).ToArray();
                case DataType.SaveableData:
                    byte[] serializedData = Serialize(data.GetValue<SaveableData>());
                    return new byte[] { (byte)DataType.SaveableData }.Concat(BitConverter.GetBytes(serializedData.Length)).Concat(serializedData).ToArray();
                case DataType.List_Int:
                    List<int> intList = data.GetValue<List<int>>();
                    var intBytes = new List<byte> { (byte)DataType.List_Int };
                    intBytes.AddRange(intList.Count.IntToBytes());
                    intList.ForEach(i => intBytes.AddRange(i.IntToBytes()));
                    return intBytes.ToArray();

                case DataType.List_Float:
                    List<float> floatList = data.GetValue<List<float>>();
                    var floatBytes = new List<byte> { (byte)DataType.List_Float };
                    floatBytes.AddRange(floatList.Count.IntToBytes());
                    floatList.ForEach(f => floatBytes.AddRange(f.FloatToBytes()));
                    return floatBytes.ToArray();

                case DataType.List_String:
                    List<string> stringList = data.GetValue<List<string>>();
                    var stringBytes = new List<byte> { (byte)DataType.List_String };
                    stringBytes.AddRange(stringList.Count.IntToBytes());
                    stringList.ForEach(s => {
                        var strBytes = System.Text.Encoding.UTF8.GetBytes(s);
                        stringBytes.AddRange(strBytes.Length.IntToBytes());
                        stringBytes.AddRange(strBytes);
                    });
                    return stringBytes.ToArray();

                case DataType.List_Long:
                    List<long> longList = data.GetValue<List<long>>();
                    var longBytes = new List<byte> { (byte)DataType.List_Long };
                    longBytes.AddRange(longList.Count.IntToBytes());
                    longList.ForEach(l => longBytes.AddRange(l.LongToBytes()));
                    return longBytes.ToArray();

                case DataType.List_Double:
                    List<double> doubleList = data.GetValue<List<double>>();
                    var doubleBytes = new List<byte> { (byte)DataType.List_Double };
                    doubleBytes.AddRange(doubleList.Count.IntToBytes());
                    doubleList.ForEach(d => doubleBytes.AddRange(d.DoubleToBytes()));
                    return doubleBytes.ToArray();

                case DataType.List_Bool:
                    List<bool> boolList = data.GetValue<List<bool>>();
                    var boolBytes = new List<byte> { (byte)DataType.List_Bool };
                    boolBytes.AddRange(boolList.Count.IntToBytes());
                    boolList.ForEach(b => boolBytes.Add(Convert.ToByte(b)));
                    return boolBytes.ToArray();

                case DataType.List_Vector3:
                    List<Vector3> vector3List = data.GetValue<List<Vector3>>();
                    var vector3Bytes = new List<byte> { (byte)DataType.List_Vector3 };
                    vector3Bytes.AddRange(vector3List.Count.IntToBytes());
                    vector3List.ForEach(v => {
                        vector3Bytes.AddRange(v.Vector3ToBytes());
                    });
                    return vector3Bytes.ToArray();

                case DataType.List_Vector2:
                    List<Vector2> vector2List = data.GetValue<List<Vector2>>();
                    var vector2Bytes = new List<byte> { (byte)DataType.List_Vector2 };
                    vector2Bytes.AddRange(vector2List.Count.IntToBytes());
                    vector2List.ForEach(v => {
                        vector2Bytes.AddRange(v.Vector2ToBytes());
                    });
                    return vector2Bytes.ToArray();

                case DataType.List_Color:
                    List<Color> colorList = data.GetValue<List<Color>>();
                    var colorBytes = new List<byte> { (byte)DataType.List_Color };
                    colorBytes.AddRange(colorList.Count.IntToBytes());
                    colorList.ForEach(c => {
                        colorBytes.AddRange(c.ColorToBytes());
                    });
                    return colorBytes.ToArray();

                case DataType.List_Quaternion:
                    List<Quaternion> quaternionList = data.GetValue<List<Quaternion>>();
                    var quaternionBytes = new List<byte> { (byte)DataType.List_Quaternion };
                    quaternionBytes.AddRange(quaternionList.Count.IntToBytes());
                    quaternionList.ForEach(q => {
                        quaternionBytes.AddRange(q.QuaternionToBytes());
                    });
                    return quaternionBytes.ToArray();

                case DataType.List_DateTime:
                    List<DateTime> dateTimeList = data.GetValue<List<DateTime>>();
                    var dateTimeBytes = new List<byte> { (byte)DataType.List_DateTime };
                    dateTimeBytes.AddRange(dateTimeList.Count.IntToBytes());
                    dateTimeList.ForEach(dt => {
                        dateTimeBytes.AddRange(dt.DateTimeToBytes());
                    });
                    return dateTimeBytes.ToArray();

                case DataType.List_SaveableData:
                    List<SaveableData> saveableDataList = data.GetValue<List<SaveableData>>();
                    var saveableDataBytes = new List<byte> { (byte)DataType.List_SaveableData };
                    saveableDataBytes.AddRange(saveableDataList.Count.IntToBytes());
                    saveableDataList.ForEach(sd => {
                        byte[] serializedData = Serialize(sd);
                        saveableDataBytes.AddRange(serializedData.Length.IntToBytes());
                        saveableDataBytes.AddRange(serializedData);
                    });
                    return saveableDataBytes.ToArray();

                default:
                    throw new InvalidOperationException("Unsupported data type." + data+" "+data.GetType());
            }
        }

        public static SaveableData Deserialize(byte[] data)
        {
            SaveableData saveableData = new SaveableData();
            int offset = 0;

            int fieldCount = data.BytesToInt(ref offset);

            for (int i = 0; i < fieldCount; i++)
            {
                // Extract the field name
                int fieldNameLength = data.BytesToInt(ref offset);
                string fieldName = System.Text.Encoding.UTF8.GetString(data, offset, fieldNameLength);
                offset += fieldNameLength;

                // Determine data type
                DataType dataType = (DataType)data[offset];
                offset++;

                object fieldValue;

                switch (dataType)
                {
                    case DataType.Int:
                        fieldValue = data.BytesToInt(ref offset);
                        break;
                    case DataType.Float:
                        fieldValue = data.BytesToFloat(ref offset);
                        break;
                    case DataType.Long:
                        fieldValue = data.BytesToLong(ref offset);
                        break;
                    case DataType.Double:
                        fieldValue = data.BytesToDouble(ref offset);
                        break;
                    case DataType.Bool:
                        fieldValue = data.BytesToBool(ref offset);
                        break;
                    case DataType.String:
                        int strLength = data.BytesToInt(ref offset);
                        fieldValue = System.Text.Encoding.UTF8.GetString(data, offset, strLength);
                        offset += strLength;
                        break;
                    case DataType.Vector3:
                        fieldValue = data.GetVector3Bytes(ref offset);
                        break;
                    case DataType.Vector2:
                        fieldValue = data.GetVector2Bytes(ref offset);
                        break;
                    case DataType.Color:
                        fieldValue = data.GetColorBytes(ref offset);
                        break;
                    case DataType.Quaternion:
                        fieldValue = data.GetQuaternionBytes(ref offset);
                        break;
                    case DataType.DateTime:
                        fieldValue = data.GetDateTimeBytes(ref offset);
                        break;
                    case DataType.SaveableData:
                        int saveableDataLength = data.BytesToInt(ref offset);
                        byte[] saveableDataBytes = new byte[saveableDataLength];
                        Array.Copy(data, offset, saveableDataBytes, 0, saveableDataLength);
                        offset += saveableDataLength;

                        fieldValue = Deserialize(saveableDataBytes);
                        break;
                    case DataType.List_Int:
                        var intList = new List<int>();
                        int listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            intList.Add(data.BytesToInt(ref offset));
                        }
                        fieldValue = intList;
                        break;

                    case DataType.List_Float:
                        var floatList = new List<float>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            floatList.Add(data.BytesToFloat(ref offset));
                        }
                        fieldValue = floatList;
                        break;

                    case DataType.List_String:
                        var stringList = new List<string>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            int strLengthTemp = data.BytesToInt(ref offset);

                            stringList.Add(System.Text.Encoding.UTF8.GetString(data, offset, strLengthTemp));
                            offset += strLengthTemp;
                        }
                        fieldValue = stringList;
                        break;

                    case DataType.List_Long:
                        var longList = new List<long>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            longList.Add(data.BytesToLong(ref offset));
                        }
                        fieldValue = longList;
                        break;

                    case DataType.List_Double:
                        var doubleList = new List<double>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            doubleList.Add(data.BytesToDouble(ref offset));
                        }
                        fieldValue = doubleList;
                        break;

                    case DataType.List_Bool:
                        var boolList = new List<bool>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            boolList.Add(data.BytesToBool(ref offset));
                        }
                        fieldValue = boolList;
                        break;
                    case DataType.List_SaveableData:
                        var saveableDataList = new List<SaveableData>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            int saveableDataLengthTemp = data.BytesToInt(ref offset);
                            byte[] saveableDataBytesTemp = new byte[saveableDataLengthTemp];
                            Array.Copy(data, offset, saveableDataBytesTemp, 0, saveableDataLengthTemp);
                            offset += saveableDataLengthTemp;

                            var saveableDataTemp = Deserialize(saveableDataBytesTemp);
                            saveableDataList.Add(saveableDataTemp);
                        }
                        fieldValue = saveableDataList;
                        break;
                    case DataType.List_Vector3:
                    case DataType.List_Vector2:
                    case DataType.List_Color:
                    case DataType.List_Quaternion:
                    case DataType.List_DateTime:
                        listCount = data.BytesToInt(ref offset);

                        List<byte[]> listData = new List<byte[]>();
                        for (int j = 0; j < listCount; j++)
                        {
                            listData.Add(DeserializeSingleItemOfList(data, ref offset, dataType));
                        }
                        fieldValue = listData;
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported data type: {dataType}");
                }

                saveableData.Fields[fieldName] = new DataWrapper(fieldValue, dataType);
            }

            return saveableData;
        }

        private static byte[] DeserializeSingleItemOfList(byte[] data, ref int offset, DataType dataType)
        {
            switch (dataType)
            {
    
                case DataType.List_Vector3:
                    return data.GetVector3Bytes(ref offset);
                case DataType.List_Vector2:
                    return data.GetVector2Bytes(ref offset);
                case DataType.List_Color:
                    return data.GetColorBytes(ref offset);
                case DataType.List_Quaternion:
                    return data.GetQuaternionBytes(ref offset);
                case DataType.List_DateTime:
                    return data.GetDateTimeBytes(ref offset);
                default:
                    throw new InvalidOperationException($"Unsupported data type: {dataType}");
            }
        }


    }


}
