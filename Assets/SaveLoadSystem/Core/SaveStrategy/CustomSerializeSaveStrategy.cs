using System;
using System.Collections.Generic;
using System.IO;


namespace SaveLoadSystem.Core
{
    internal class CustomSerializeSaveStrategy : ISaveLoadStrategy
    {
        // Property to get the file extension for save files.
        public string FileExtension => ".cus";

        /// <summary>
        /// Saves the specified data to a file, with options for encryption.
        /// </summary>
        /// <param name="savable">The data to be saved.</param>
        /// <param name="path">The directory path where the file will be saved.</param>
        /// <param name="fileName">The name of the file to save.</param>
        /// <param name="encryptionType">Optional flag to determine if the data should be encrypted. Default is none.</param>
        /// <param name="encryptionKey">Optional encryption key used when encrypting the data.</param>
        public void Save(SavableData savable, string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "")
        {
            // Append the custom file extension to the file name.
            fileName += FileExtension;
            // Combine the directory path and file name into a full path.
            path = Path.Combine(path, fileName);

            // Initialize a list to hold serialized data bytes.
            List<byte> serializedList = new List<byte>();
            // Serialize the savable data into the list.
            Serialize(savable, serializedList, runAsync);
            // Convert the list of bytes to an array for file writing.
            byte[] serializedData = serializedList.ToArray();
            // Check if encryption is requested.
            if (encryptionType != EncryptionType.None)
            {
                // Encrypt the serialized data using the provided encryption key.
                serializedData = EncryptionHelper.Encrypt(serializedData, encryptionType, encryptionKey);
            }
            // Write the serialized data to the file.
            File.WriteAllBytes(path, serializedData);
        }

        /// <summary>
        /// Loads data from a file, with options for decryption.
        /// </summary>
        /// <param name="path">The directory path where the file is located.</param>
        /// <param name="fileName">The name of the file to load.</param>
        /// <param name="encryptionType">Optional flag to determine if the data should be decrypted. Default is none.</param>
        /// <param name="encryptionKey">Optional decryption key used when decrypting the data.</param>
        /// <returns>A SavableData object containing the loaded data, or null if the file does not exist or the load fails.</returns>
        public SavableData Load(string path, string fileName, bool runAsync = false, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "")
        {
            // Append the custom file extension to the file name.
            fileName += FileExtension;
            // Combine the directory path and file name into a full path.
            path = Path.Combine(path, fileName);

            // Check if the file exists at the specified path.
            if (!File.Exists(path))
            {
                // Log an error message if the file does not exist.
                UnityEngine.Debug.LogError("Save file not found at " + path);
                // Return null to indicate that loading failed.
                return null;
            }

            // Read all bytes from the save file.
            byte[] serializedData = File.ReadAllBytes(path);

            // Check if decryption is requested.
            if (encryptionType != EncryptionType.None)
            {
                // Decrypt the serialized data using the provided decryption key.
                serializedData = EncryptionHelper.Decrypt(serializedData, encryptionType, encryptionKey);
            }
            // Initialize an offset variable for tracking the read position in the byte array.
            int offset = 0;
            // Deserialize the byte array back into a SavableData object and return it.
            return Deserialize(serializedData, ref offset, runAsync);
        }




        /// <summary>
        /// Serializes the provided SavableData object into a list of bytes.
        /// </summary>
        /// <param name="savableData">The SavableData object to be serialized.</param>
        /// <param name="serializedData">The list of bytes where the serialized data will be stored.</param>
        private static void Serialize(SavableData savableData, List<byte> serializedData, bool runAsync)
        {
            // Retrieve the dictionary of data fields from the savableData object.
            Dictionary<string, DataWrapper> fields = savableData.Fields;

            // Convert the count of fields into bytes and add to the serialized data list.
            serializedData.AddRange(fields.Count.IntToBytes());

            // Iterate through each item in the fields dictionary.
            foreach (var item in fields)
            {
                // Convert the field name (key) into bytes using UTF8 encoding.
                byte[] fieldNameBytes = System.Text.Encoding.UTF8.GetBytes(item.Key);
                // Convert the length of the field name into bytes and add to the serialized data list.
                serializedData.AddRange(fieldNameBytes.Length.IntToBytes());
                // Add the field name bytes to the serialized data list.
                serializedData.AddRange(fieldNameBytes);
                // Serialize the field value (DataWrapper) and add the serialized bytes to the list.
                // ConvertToByteAndAdd handles the conversion of various data types to bytes.
                ConvertToByteAndAdd(item.Value, serializedData, runAsync);
            }

            // The method does not return a value as it directly modifies the provided serializedData list.
        }



        /// <summary>
        /// Converts various data types into a byte representation and adds them to a referenced byte list.
        /// </summary>
        /// <param name="data">The data to be converted into bytes.</param>
        /// <param name="refSerializedData">The referenced list of bytes where the converted data will be added.</param>
        private static void ConvertToByteAndAdd(DataWrapper data, List<byte> refSerializedData, bool runAsync)
        {
            // Switch statement to handle different data types.
            switch (data.Type)
            {
                case DataType.Int:
                    // Add the DataType byte and the integer value converted to bytes.
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    refSerializedData.AddRange(data.GetValue<int>().IntToBytes());
                    return;
                case DataType.String:
                    // Add the DataType byte for a string.
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    // Get the string value from the DataWrapper and convert the length of the string into bytes and add to the list.
                    refSerializedData.AddRange(data.GetValue<string>().Length.IntToBytes());
                    // Convert the string into bytes (UTF8 encoding) and add to the list.
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
                case DataType.SavableData:
                    // Add the DataType byte for SavableData.
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    // Serialize the SavableData object and add the serialized bytes to the list.
                    Serialize(data.GetValue<SavableData>(), refSerializedData, runAsync);
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
                        string str = stringList[i];
                        if(str == null)
                        {
                            str = string.Empty;
                        }
                        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                        refSerializedData.AddRange(str.Length.IntToBytes());
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

                case DataType.List_SavableData:
                    // Handle a list of SavableData objects.
                    List<SavableData> savableDataList = data.GetValue<List<SavableData>>();
                    // Add the DataType byte for a list of savableData objects.
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    // Convert the count of savableData objects in the list into bytes and add to the list.
                    refSerializedData.AddRange(savableDataList.Count.IntToBytes());
                    // Serialize each savableData object in the list and add the bytes to the list.
                    for (int i = 0; i < savableDataList.Count; i++)
                    {
                        Serialize(savableDataList[i], refSerializedData, runAsync);
                    }
                    return;

                case DataType.Null:
                    // Add the DataType byte for null.
                    refSerializedData.Add(data.Type.DataTypeToByte());
                    return;


                default:
                    throw new InvalidOperationException("Unsupported data type." + data+" "+data.GetType());
            }
        }


        /// <summary>
        /// Deserializes the provided byte array into a SavableData object.
        /// </summary>
        /// <param name="data">The byte array to be deserialized.</param>
        /// <param name="offset">The reference to the current position in the byte array.</param>
        /// <returns>A SavableData object reconstructed from the byte array.</returns>
        private static SavableData Deserialize(byte[] data, ref int offset, bool runAsync)
        {
            // Create a new SavableData object to store the deserialized data.
            SavableData savableData = new SavableData();

            // Extract the number of fields from the byte array and update the offset.
            int fieldCount = data.BytesToInt(ref offset);

            // Iterate over each field based on the field count.
            for (int i = 0; i < fieldCount; i++)
            {
                // Deserialize the field name.
                int fieldNameLength = data.BytesToInt(ref offset);
                string fieldName = System.Text.Encoding.UTF8.GetString(data, offset, fieldNameLength);
                offset += fieldNameLength;


                // Determine the data type of the field.
                DataType dataType = (DataType)data[offset];
                offset++;

                object fieldValue;
                // Switch statement to handle different data types.
                // Each case deserializes a specific data type from the byte array.
                // The deserialization process depends on the data type, and the offset is updated accordingly.
                // For basic data types like Int, Float, Long, Double, Bool, the respective 'BytesToX' method is called.
                // Special handling for strings: extract the string length and then the string itself.
                // Recursive call for nested SavableData objects.
                // For lists of different DataTypes, the process involves reading the list count and then deserializing
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
                    case DataType.SavableData:
                        fieldValue = Deserialize(data, ref offset, runAsync);
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
                    case DataType.List_SavableData:
                        var savableDataList = new List<SavableData>();
                        listCount = data.BytesToInt(ref offset);
                        for (int j = 0; j < listCount; j++)
                        {
                            var savableDataTemp = Deserialize(data, ref offset, runAsync);
                            savableDataList.Add(savableDataTemp);
                        }
                        fieldValue = savableDataList;
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

                    case DataType.Null:
                        fieldValue = null;
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported data type: {dataType}");
                }

                savableData.Fields[fieldName] = new DataWrapper(fieldValue, dataType);
            }

            return savableData;
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
