using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine;

public class Test : MonoBehaviour, ISaveable
{


    public SaveMode saveMode;


    [ContextMenu("Save")]
    public void Save()
    {

        SaveLoadSystem.Save(this, "SaveSlot1", saveMode);
    }

    [ContextMenu("Load")]
    public void Load()
    {

        SaveableDataWrapper loadedData = SaveLoadSystem.Load("SaveSlot1", saveMode);
        LoadSavedData(loadedData);
    }
    public void LoadSavedData(SaveableDataWrapper data)
    {

        Debug.Log(data.Read<long>("longValue"));
        Debug.Log(data.Read<double>("doubleValue"));
        Debug.Log(data.Read<Color>("color"));
        Debug.Log(data.Read<Vector2>("vector2"));
        Debug.Log(data.Read<Quaternion>("q"));
        Debug.Log(data.Read<DateTime>("dateTime"));

        int[] array = data.Read<int[]>("array");

        foreach (var item in array)
        {
            Debug.Log("array : " + item);
        }

    }

    public SaveableDataWrapper CreateSaveData()
    {

        SaveableDataWrapper saveData = new SaveableDataWrapper();

        saveData.Write("longValue", 9999999L);
        saveData.Write("doubleValue", 9999999.88D);
        saveData.Write("color", new Color(0, 50, 0));
        saveData.Write("q", transform.rotation);
        saveData.Write("dateTime", System.DateTime.Now);

        saveData.Write("vector2", new Vector2(10, 20));

        int[] array = new int[20];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i * 3;
        }

        saveData.Write("array", array);

        return saveData;
    }




}
