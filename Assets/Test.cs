using Newtonsoft.Json;
using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour, ISaveable
{


    public SaveMode saveMode;
    public string fileName = "SaveSlot1";


    [ContextMenu("Save")]
    public void Save()
    {
        SaveLoadManager.Save(this, fileName, saveMode);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        SaveableData loadedData = SaveLoadManager.Load("SaveSlot1", saveMode);
        LoadSavedData(loadedData);
    }


    [ContextMenu("Test")]
    void TestMethod()
    {
        // Create a new SaveableData instance and write various types of data
        SaveableData saveData = new SaveableData();

        // Writing basic data types
        saveData.Write("longValue", 9999999L);
        saveData.Write("doubleValue", 9999999.88D);
        saveData.Write("color", new Color(0, 50, 0));
        saveData.Write("quaternion", transform.rotation);
        saveData.Write("dateTime", System.DateTime.Now);

        // Writing complex data types
        saveData.Write("vector2", new Vector2(10, 20));

        // Writing a list of basic data types
        List<int> intList = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            intList.Add(i);
        }
        saveData.Write("listInt", intList);

        // Writing a list of complex data types
        List<Vector3> vectorList = new List<Vector3>();
        for (int i = 0; i < 5; i++)
        {
            vectorList.Add(new Vector3(i, i * 2, i * 3));
        }
        saveData.Write("listVector3", vectorList);


        SaveLoadManager.Save(saveData, fileName, saveMode);


        SaveableData deserializedSaveData = SaveLoadManager.Load(fileName, saveMode);

        // Verify that deserialized data matches original data
        Debug.Assert(saveData.Read<long>("longValue") == deserializedSaveData.Read<long>("longValue"), "Long value mismatch");
        Debug.Assert(saveData.Read<double>("doubleValue") == deserializedSaveData.Read<double>("doubleValue"), "Double value mismatch");
        Debug.Assert(saveData.Read<Color>("color").Equals(deserializedSaveData.Read<Color>("color")), "Color value mismatch");
        Debug.Assert(saveData.Read<Quaternion>("quaternion").Equals(deserializedSaveData.Read<Quaternion>("quaternion")), "Quaternion value mismatch");
        Debug.Assert(saveData.Read<DateTime>("dateTime").Equals(deserializedSaveData.Read<DateTime>("dateTime")), "DateTime value mismatch");

        // Verify complex types
        Debug.Assert(saveData.Read<Vector2>("vector2").Equals(deserializedSaveData.Read<Vector2>("vector2")), "Vector2 value mismatch");

        // Verify lists
        List<int> originalIntList = saveData.Read<List<int>>("listInt");
        List<int> deserializedIntList = deserializedSaveData.Read<List<int>>("listInt");
        Debug.Assert(originalIntList.SequenceEqual(deserializedIntList), "List<int> value mismatch");

        List<Vector3> originalVectorList = saveData.Read<List<Vector3>>("listVector3");
        List<Vector3> deserializedVectorList = deserializedSaveData.Read<List<Vector3>>("listVector3");
        Debug.Log(originalVectorList + " " + deserializedVectorList);
        Debug.Assert(originalVectorList.SequenceEqual(deserializedVectorList), "List<Vector3> value mismatch");

        // Output result
        Debug.Log("All tests passed!");
    }



    [ContextMenu("Test2")]
    void TestComplexSaveableData()
    {
        // Root SaveableData
        SaveableData rootSaveData = new SaveableData();
        rootSaveData.Write("rootLong", 123456789L);
        rootSaveData.Write("rootString", "RootLevelData");

        // Nested SaveableData
        SaveableData nestedSaveData = new SaveableData();
        nestedSaveData.Write("nestedInt", 987);
        nestedSaveData.Write("nestedBool", true);

        // Adding the nested SaveableData to the root
        rootSaveData.Write("nestedData", nestedSaveData);

        // Creating a list of SaveableData for complexity
        List<SaveableData> listOfSaveableData = new List<SaveableData>();
        for (int i = 0; i < 3; i++)
        {
            SaveableData listSaveableData = new SaveableData();
            listSaveableData.Write("index", i);
            listSaveableData.Write("nestedString", $"Item{i}");
            listOfSaveableData.Add(listSaveableData);
        }

        // Writing the list of SaveableData to the root
        rootSaveData.Write("listOfNestedData", listOfSaveableData);

        // Sub-nested SaveableData for even more complexity
        SaveableData subNestedSaveData = new SaveableData();
        subNestedSaveData.Write("subNestedFloat", 3.14f);

        // Adding a sub-nested SaveableData to one of the list items
        listOfSaveableData[1].Write("subNestedData", subNestedSaveData);

        SaveLoadManager.Save(rootSaveData, fileName, saveMode);


        SaveableData deserializedRootSaveData = SaveLoadManager.Load(fileName, saveMode);

        // Verification: Check that the root level data matches
        Debug.Assert(rootSaveData.Read<long>("rootLong") == deserializedRootSaveData.Read<long>("rootLong"), "Root level long value mismatch");
        Debug.Assert(rootSaveData.Read<string>("rootString") == deserializedRootSaveData.Read<string>("rootString"), "Root level string value mismatch");

        // Verification: Check that the nested SaveableData is correct
        SaveableData deserializedNestedData = deserializedRootSaveData.Read<SaveableData>("nestedData");
        Debug.Assert(nestedSaveData.Read<int>("nestedInt") == deserializedNestedData.Read<int>("nestedInt"), "Nested int value mismatch");
        Debug.Assert(nestedSaveData.Read<bool>("nestedBool") == deserializedNestedData.Read<bool>("nestedBool"), "Nested bool value mismatch");

        // Verification: Check that the list of SaveableData is correct
        List<SaveableData> deserializedListOfSaveableData = deserializedRootSaveData.Read<List<SaveableData>>("listOfNestedData");
        for (int i = 0; i < listOfSaveableData.Count; i++)
        {
            Debug.Assert(listOfSaveableData[i].Read<int>("index") == deserializedListOfSaveableData[i].Read<int>("index"), $"List item {i} index value mismatch");
            Debug.Assert(listOfSaveableData[i].Read<string>("nestedString") == deserializedListOfSaveableData[i].Read<string>("nestedString"), $"List item {i} string value mismatch");
        }

        // Verification: Check the sub-nested SaveableData
        SaveableData deserializedSubNestedData = deserializedListOfSaveableData[1].Read<SaveableData>("subNestedData");
        Debug.Assert(subNestedSaveData.Read<float>("subNestedFloat") == deserializedSubNestedData.Read<float>("subNestedFloat"), "Sub-nested float value mismatch");

        // Output result
        Debug.Log("All complex tests passed!");
    }


    [ContextMenu("Test2")]
    void TestComplex2SaveableData()
    {
        // Root SaveableData
        SaveableData rootSaveData = new SaveableData();
        SaveableData f0 = new SaveableData();
        SaveableData f1 = new SaveableData();
        SaveableData f0_0 = new SaveableData();
        SaveableData f0_0_1 = new SaveableData();
        SaveableData f1_0 = new SaveableData();
        SaveableData f1_1 = new SaveableData();
        SaveableData f1_2 = new SaveableData();

        List<SaveableData> list = new List<SaveableData>();
        SaveableData listNode0 = new SaveableData();
        SaveableData listNode1 = new SaveableData();
        SaveableData listNode2 = new SaveableData();

        SaveableData listNode1_0 = new SaveableData();

        listNode1_0.Write("f0", 45);

        listNode0.Write("f0", 10);
        listNode1.Write("f0", listNode1_0);
        listNode2.Write("f0", 25);
        list.Add(listNode0);
        list.Add(listNode1);
        list.Add(listNode2);



        rootSaveData.Write("f0", f0);

        f0.Write("f0", f0_0);
        f0_0.Write("f0", 20);
        f0_0.Write("f1", f0_0_1);
        f0_0_1.Write("f0", list);

        rootSaveData.Write("f1", f1);

        f1.Write("f0", f1_0);
        f1.Write("f1", f1_1);
        f1.Write("f2", f1_2);

        f1_0.Write("f0", 5);
        f1_1.Write("f1", 30);
        f1_2.Write("f2", "Hi");


        SaveLoadManager.Save(rootSaveData, fileName, saveMode);


        SaveableData deserializedRootSaveData = SaveLoadManager.Load(fileName, saveMode);

        // Begin testing assertions
        Debug.Assert(deserializedRootSaveData.Read<SaveableData>("f0").Read<SaveableData>("f0").Read<int>("f0") == 20, "Nested int value mismatch");
        Debug.Assert(deserializedRootSaveData.Read<SaveableData>("f0").Read<SaveableData>("f0").Read<SaveableData>("f1").Read<List<SaveableData>>("f0").Count == list.Count, "List<SaveableData> count mismatch");

        // Test the list of SaveableData
        SaveableData deserializedListNode0 = deserializedRootSaveData.Read<SaveableData>("f0").Read<SaveableData>("f0").Read<SaveableData>("f1").Read<List<SaveableData>>("f0")[0];
        SaveableData deserializedListNode1 = deserializedRootSaveData.Read<SaveableData>("f0").Read<SaveableData>("f0").Read<SaveableData>("f1").Read<List<SaveableData>>("f0")[1];
        SaveableData deserializedListNode2 = deserializedRootSaveData.Read<SaveableData>("f0").Read<SaveableData>("f0").Read<SaveableData>("f1").Read<List<SaveableData>>("f0")[2];

        Debug.Assert(deserializedListNode0.Read<int>("f0") == 10, "List node 0 value mismatch");
        Debug.Assert(deserializedListNode1.Read<SaveableData>("f0").Read<int>("f0").Equals(listNode1_0.Read<int>("f0")), "List node 1 SaveableData mismatch");
        Debug.Assert(deserializedListNode2.Read<int>("f0") == 25, "List node 2 value mismatch");

        // Test the direct children of root
        Debug.Assert(deserializedRootSaveData.Read<SaveableData>("f1").Read<SaveableData>("f0").Read<int>("f0") == 5, "Direct child f1_0 value mismatch");
        Debug.Assert(deserializedRootSaveData.Read<SaveableData>("f1").Read<SaveableData>("f1").Read<int>("f1") == 30, "Direct child f1_1 value mismatch");
        Debug.Assert(deserializedRootSaveData.Read<SaveableData>("f1").Read<SaveableData>("f2").Read<string>("f2").Equals("Hi"), "Direct child f1_2 value mismatch");

        // Output result
        Debug.Log("All complex structure tests passed!");



    }


    public void LoadSavedData(SaveableData data)
    {

        Debug.Log(data.Read<long>("longValue"));
        Debug.Log(data.Read<double>("doubleValue"));
        Debug.Log(data.Read<Color>("color"));
        Debug.Log(data.Read<Vector2>("vector2"));
        Debug.Log(data.Read<Quaternion>("q"));
        Debug.Log(data.Read<DateTime>("dateTime"));

        List<int> array = data.Read<List<int>>("list");

        foreach (var item in array)
        {
            Debug.Log("array : " + item);
        }

    }

    public SaveableData CreateSaveData()
    {

        SaveableData saveData = new SaveableData();

        saveData.Write("longValue", 9999999L);
        saveData.Write("doubleValue", 9999999.88D);
        saveData.Write("color", new Color(0, 50, 0));
        saveData.Write("q", transform.rotation);
        saveData.Write("dateTime", System.DateTime.Now);

        saveData.Write("vector2", new Vector2(10, 20));

        List<int> array = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            array.Add(i);
        }

        saveData.Write("list", array);

        return saveData;
    }




}
