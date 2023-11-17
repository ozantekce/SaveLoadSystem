
using SaveLoadSystem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class Schema
{

    public Schema()
    { 
    
    }


    public Schema(SaveableData data)
    {
        ConvertToSchema(data);
    }


    public static SaveableData ConvertToSaveableData(Schema schema)
    {
        SaveableData data = new SaveableData();

        foreach (FieldInfo field in schema.GetType().GetFields())
        {
            var value = field.GetValue(schema);
            if (value is Schema schemaValue)
            {
                SaveableData s = ConvertToSaveableData(schemaValue);
                data.Write(field.Name, s);
            }
            else if (value is IList list && value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                List<SaveableData> saveableList = new List<SaveableData>();
                foreach (var item in list)
                {
                    if (item is Schema listItem)
                    {
                        SaveableData listItemData = ConvertToSaveableData(listItem);
                        saveableList.Add(listItemData);
                    }
                }
                data.Write(field.Name, saveableList);
            }
            else
            {
                data.Write(field.Name, value);
            }
        }

        return data;
    }


    public SaveableData ConvertToSaveableData()
    {
        SaveableData data = new SaveableData();

        foreach (FieldInfo field in this.GetType().GetFields())
        {
            var value = field.GetValue(this);

            if(value == null)
            {

            }
            else if (value is Schema schemaValue)
            {
                SaveableData s = ConvertToSaveableData(schemaValue);
                data.Write(field.Name, s);
            }
            else if (value is IList list && value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                List<SaveableData> saveableList = new List<SaveableData>();
                foreach (var item in list)
                {
                    if (item is Schema listItem)
                    {
                        SaveableData listItemData = ConvertToSaveableData(listItem);
                        saveableList.Add(listItemData);
                    }
                }
                data.Write(field.Name, saveableList);
            }
            else
            {
                data.Write(field.Name, value);
            }
        }

        return data;
    }



    public void ConvertToSchema(SaveableData data)
    {
        foreach (FieldInfo field in this.GetType().GetFields())
        {
            if (data.TryRead(field.Name, out var loadedValue))
            {
                if (loadedValue is SaveableData saveableData)
                {
                    Type fieldType = field.FieldType;
                    object fieldInstance = Activator.CreateInstance(fieldType, new object[] { saveableData });
                    field.SetValue(this, fieldInstance);
                }
                else if (loadedValue is IList && loadedValue.GetType().IsGenericType && loadedValue.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                {
                    Type itemType = field.FieldType.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var listInstance = (IList)Activator.CreateInstance(listType);

                    foreach (var item in (IList)loadedValue)
                    {
                        if (item is SaveableData itemData)
                        {
                            var itemInstance = Activator.CreateInstance(itemType, new object[] { itemData });
                            listInstance.Add(itemInstance);
                        }
                    }

                    field.SetValue(this, listInstance);
                }
                else
                {
                    field.SetValue(this, loadedValue);
                }
            }
        }
    }




}
