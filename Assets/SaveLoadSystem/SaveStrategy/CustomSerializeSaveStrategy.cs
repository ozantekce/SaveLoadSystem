using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static SaveLoadSystem.DataWrapper;
using static UnityEditor.Progress;

namespace SaveLoadSystem
{
    public class CustomSerializeSaveStrategy : ISaveLoadStrategy
    {
        public string FileExtension => ".cus";

        public void Save(SaveableData saveableData, string path, string fileName, bool encrypt = false, string encryptionKey = null)
        {
            fileName += FileExtension;
            path = Path.Combine(path, fileName);
            
            List<byte> serializedList = new List<byte>();
            Serialize(saveableData, serializedList);
            byte[] serializedData = serializedList.ToArray();
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


        public struct FieldNameChildPair
        {
            public string name;
            public DataWrapper child;

            public FieldNameChildPair(string name, DataWrapper child)
            {
                this.name = name;
                this.child = child;
            }
        }

        public struct FieldNameChildsListPair
        {
            public string name;
            public DataWrapper childsList;

            public FieldNameChildsListPair(string name, DataWrapper childsList)
            {
                this.name = name;
                this.childsList = childsList;
            }
        }


        public static void Serialize(SaveableData saveableData, List<byte> serializedData)
        {
            //List<byte> serializedData = new List<byte>();
            Dictionary<string, DataWrapper> fields = saveableData.Fields;

            serializedData.AddRange(BitConverter.GetBytes(fields.Count));



            foreach (var item in fields)
            {
                /*
                if(item.Value.Type == DataType.SaveableData)
                {
                    childs.Enqueue(new FieldNameChildPair(item.Key, item.Value));
                    continue;
                }
                else if (item.Value.Type == DataType.List_SaveableData)
                {
                    childsLists.Enqueue(new FieldNameChildsListPair(item.Key, item.Value));
                    continue;
                }
                */

                byte[] fieldNameBytes = System.Text.Encoding.UTF8.GetBytes(item.Key);

                serializedData.AddRange(fieldNameBytes.Length.IntToBytes());
                serializedData.AddRange(fieldNameBytes);

                ConvertToByteAndAdd(item.Value, serializedData);


            }

            /*
            while(childs.Count > 0)
            {
                FieldNameChildPair current = childs.Dequeue();
                string fieldName = current.name;
                DataWrapper sd = current.child;

                byte[] fieldNameBytes = System.Text.Encoding.UTF8.GetBytes(fieldName);
                serializedData.AddRange(BitConverter.GetBytes(fieldNameBytes.Length));
                serializedData.AddRange(fieldNameBytes);
                ConvertToByteAndAdd(sd, serializedData, ref offset);
            }

            while (childsLists.Count > 0)
            {
                FieldNameChildsListPair current = childsLists.Dequeue();
                string fieldName = current.name;
                DataWrapper sd = current.childsList;

                byte[] fieldNameBytes = System.Text.Encoding.UTF8.GetBytes(fieldName);
                serializedData.AddRange(BitConverter.GetBytes(fieldNameBytes.Length));
                serializedData.AddRange(fieldNameBytes);
                ConvertToByteAndAdd(sd, serializedData, ref offset);
            }
            */

            return;
        }

        private static void ConvertToByteAndAdd(DataWrapper data, List<byte> refSerializedData)
        {
            switch (data.Type)
            {
                case DataType.Int:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.GetValue<int>().IntToBytes());
                    return;
                case DataType.String:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.GetValue<string>().Length.IntToBytes());
                    refSerializedData.AddRange(System.Text.Encoding.UTF8.GetBytes(data.GetValue<string>()));
                    return;
                case DataType.Float:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.GetValue<float>().FloatToBytes());
                    return;
                case DataType.Long:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.GetValue<long>().LongToBytes());
                    return;
                case DataType.Double:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.GetValue<double>().DoubleToBytes());

                    return;
                case DataType.Bool:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.Add(Convert.ToByte(data.GetValue<bool>()));
                    return;
                case DataType.Vector3:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.Bytes);
                    return;
                case DataType.Vector2:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.Bytes);

                    return;
                case DataType.Color:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.Bytes);
                    return;
                case DataType.Quaternion:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.Bytes);
                    return;
                case DataType.DateTime:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.Bytes);
                    return;
                case DataType.SaveableData:
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    int currentOffset = refSerializedData.Count;
                    Serialize(data.GetValue<SaveableData>(), refSerializedData);
                    int added = refSerializedData.Count - currentOffset;
                    refSerializedData.InsertRange(currentOffset, added.IntToBytes());
                    return;
                case DataType.List_Int:
                    List<int> intList = data.GetValue<List<int>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(intList.Count.IntToBytes());
                    intList.ForEach(i => refSerializedData.AddRange(i.IntToBytes()));
                    return;
                case DataType.List_Float:
                    List<float> floatList = data.GetValue<List<float>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(floatList.Count.IntToBytes());
                    floatList.ForEach(f => refSerializedData.AddRange(f.FloatToBytes()));
                    return;

                case DataType.List_String:
                    List<string> stringList = data.GetValue<List<string>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(stringList.Count.IntToBytes());

                    for (int i = 0; i < stringList.Count; i++)
                    {
                        var strBytes = System.Text.Encoding.UTF8.GetBytes(stringList[i]);
                        refSerializedData.AddRange(stringList[i].Length.IntToBytes());
                        refSerializedData.AddRange(strBytes);

                    }

                    return;

                case DataType.List_Long:
                    List<long> longList = data.GetValue<List<long>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(longList.Count.IntToBytes());
                    longList.ForEach(l => refSerializedData.AddRange(l.LongToBytes()));
                    return;

                case DataType.List_Double:
                    List<double> doubleList = data.GetValue<List<double>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(doubleList.Count.IntToBytes());
                    doubleList.ForEach(d => refSerializedData.AddRange(d.DoubleToBytes()));
                    return;

                case DataType.List_Bool:
                    List<bool> boolList = data.GetValue<List<bool>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(boolList.Count.IntToBytes());
                    boolList.ForEach(b => refSerializedData.Add(Convert.ToByte(b)));
                    return;

                case DataType.List_Vector3:
                    List<byte[]> vector3List = data.BytesList;
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(vector3List.Count.IntToBytes());
                    vector3List.ForEach(v => {
                        refSerializedData.AddRange(v);
                    });
                    return;

                case DataType.List_Vector2:
                    List<byte[]> vector2List = data.BytesList;
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(vector2List.Count.IntToBytes());
                    vector2List.ForEach(v => {
                        refSerializedData.AddRange(v);
                    });
                    return;

                case DataType.List_Color:
                    List<byte[]> colorList = data.BytesList;
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(colorList.Count.IntToBytes());
                    colorList.ForEach(c => {
                        refSerializedData.AddRange(c);
                    });
                    return;

                case DataType.List_Quaternion:
                    List<byte[]> quaternionList = data.BytesList;
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(quaternionList.Count.IntToBytes());
                    quaternionList.ForEach(q => {
                        refSerializedData.AddRange(q);
                    });
                    return;

                case DataType.List_DateTime:
                    List<byte[]> dateTimeList = data.BytesList;
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(dateTimeList.Count.IntToBytes());
                    dateTimeList.ForEach(dt => {
                        refSerializedData.AddRange(dt);
                    });
                    return;

                case DataType.List_SaveableData:
                    List<SaveableData> saveableDataList = data.GetValue<List<SaveableData>>();
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(saveableDataList.Count.IntToBytes());
                    for (int i = 0; i < saveableDataList.Count; i++)
                    {
                        currentOffset = refSerializedData.Count;
                        Serialize(saveableDataList[i], refSerializedData);
                        added = refSerializedData.Count - currentOffset;
                        refSerializedData.InsertRange(currentOffset, added.IntToBytes());
                    }
                    return;

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
