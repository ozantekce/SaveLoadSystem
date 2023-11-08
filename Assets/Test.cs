using Newtonsoft.Json;
using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using static SaveLoadSystem.DataWrapper;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour, ISaveable
{


    public SaveMode saveMode;
    public string fileName = "SaveSlot1";
    public int randomTestSize = 100;


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


    [ContextMenu("RandomTest")]
    public void RandomTest()
    {

        int totalCreated = 0;
        SaveableData root = CreateRandomSaveableData(randomTestSize, ref totalCreated);

        Debug.Log("total test data size : " + totalCreated);

        SaveableData deserializedSaveData;
        bool testResult;
        Stopwatch stopwatch = new Stopwatch();
        DateTime startTime, endTime;
        TimeSpan duration;

        //-------------------------------------------------------------------------
        // Custom Serialization
        stopwatch.Start();
        startTime = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.CustomSerialize);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Custom Serialization Saving takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        stopwatch.Restart();
        startTime = DateTime.Now;
        deserializedSaveData = SaveLoadManager.Load(fileName, SaveMode.CustomSerialize);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Custom Serialization Loading takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        testResult = IsSame(root, deserializedSaveData);
        Debug.Log($"Custom Serialization test result: {testResult}");
        //-------------------------------------------------------------------------
        Debug.Log("^#################################################################");

        // Binary Serialization
        stopwatch.Restart();
        startTime = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.Serialize);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Binary Saving takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        stopwatch.Restart();
        startTime = DateTime.Now;
        deserializedSaveData = SaveLoadManager.Load(fileName, SaveMode.Serialize);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Binary Loading takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        testResult = IsSame(root, deserializedSaveData);
        Debug.Log($"Binary test result: {testResult}");
        //-------------------------------------------------------------------------
        Debug.Log("^#################################################################");

        /*
        // JSON Serialization
        stopwatch.Restart();
        startTime = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.Json);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"JSON Saving takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        stopwatch.Restart();
        startTime = DateTime.Now;
        deserializedSaveData = SaveLoadManager.Load(fileName, SaveMode.Json);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"JSON Loading takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        testResult = IsSame(root, deserializedSaveData);
        Debug.Log($"JSON test result: {testResult}");
        //-------------------------------------------------------------------------
        Debug.Log("^#################################################################");
        */

    }


    private SaveableData CreateRandomSaveableData(int minCount, ref int totalCreated)
    {

        Dictionary<DataType, ProbabilityMinMaxCreateCount> dataProbabilities = new Dictionary<DataType, ProbabilityMinMaxCreateCount>
        {
            { DataType.Int, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Float, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Long, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Double, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.String, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Vector3, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Vector2, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Color, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Quaternion, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.DateTime, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.SaveableData, new ProbabilityMinMaxCreateCount(0.3f, 1, 2) },
            { DataType.List_Int, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Float, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Long, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Double, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Bool, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_String, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Vector3, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Vector2, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Color, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_Quaternion, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_DateTime, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.List_SaveableData, new ProbabilityMinMaxCreateCount(0.3f, 3, 20) }
        };


        string fieldName = "f";

        SaveableData root = new SaveableData();
        int currentCount = 0;
        Queue<SaveableData> frontier = new Queue<SaveableData>();

        frontier.Enqueue(root);

    Continue:
        while (frontier.Count > 0 && currentCount < minCount)
        {

            SaveableData current = frontier.Dequeue();

            if (totalCreated >= randomTestSize) break;

            for (int i = 0; i <= 24; i++)
            {
                DataType dt = (DataType)i;
                if (dataProbabilities.ContainsKey(dt))
                {
                    float r = UnityEngine.Random.Range(0f, 1f);
                    float p = dataProbabilities[dt].prob;
                    int min = dataProbabilities[dt].min;
                    int max = dataProbabilities[dt].max;
                    if (r < p)
                    {
                        int randomCount = UnityEngine.Random.Range(min, max);

                        for (int j = 0; j < randomCount; j++)
                        {
                            if (dt == DataType.SaveableData)
                            {
                                SaveableData child = new SaveableData();
                                frontier.Enqueue(child);
                                current.Write(fieldName + currentCount, child);
                                currentCount++;
                                totalCreated++;
                            }
                            else if(dt == DataType.List_SaveableData)
                            {
                                List<SaveableData> list = new List<SaveableData>();
                                for (int k = 0; k < 5; k++)
                                {
                                    list.Add(CreateRandomSaveableData(5,ref totalCreated));
                                }
                                current.Write(fieldName+currentCount, list);
                                currentCount++;
                                totalCreated++;
                            }
                            else
                            {
                                current.Write(fieldName + currentCount, RandomValueGenerator.CreateRandom(dt));
                                currentCount++;
                                totalCreated++;
                            }
                        }
                    }
                }
            }

        }

        if (currentCount < minCount && totalCreated < randomTestSize)
        {
            frontier.Enqueue(root);
            goto Continue;
        }

        return root;

    }


    public bool IsSame(SaveableData s0, SaveableData s1)
    {

        if (s0.Fields.Count != s1.Fields.Count)
        {
            Debug.LogError("Here");
            return false;
        }

        foreach (var pair in s0.Fields)
        {
            string key = pair.Key;

            if (!s1.Fields.TryGetValue(key, out DataWrapper d1))
            {
                Debug.LogError("Here");
                return false;
            }

            DataWrapper d0 = pair.Value;
            if (!IsSame(d0, d1))
            {
                Debug.LogError("Here : "+ d0.Value+" "+ d0.Type+" | " +" "+ d1.Value+" "+d1.Type);
                return false;
            }
        }

        return true;

    }

    public bool IsSame(DataWrapper d0, DataWrapper d1)
    {

        if(d0.Type != d1.Type) return false;
        if ((int)d0.Type < 6)
        {
            return d0.Value.Equals(d1.Value);
        }
        else if ((int)d0.Type < 11)
        {
            byte[] bytes0 = (byte[])d0.Value;
            byte[] bytes1 = (byte[])d1.Value;

            return bytes0.SequenceEqual(bytes1);
        }
        else if(d0.Type == DataType.SaveableData)
        {
            return IsSame(d0.GetValue<SaveableData>(), d1.GetValue<SaveableData>());
        }
        else
        {
            IList list0 = (IList)d0.Value;
            IList list1 = (IList)d1.Value;
            if(list0.Count != list1.Count) return false;
            for (int i = 0; i < list0.Count; i++)
            {
                if (d0.Type == DataType.List_SaveableData)
                {
                    if (!IsSame((SaveableData)list0[i], (SaveableData)list1[i])) return false;
                }
                else
                {
                    if ((int)d0.Type >= 18)
                    {
                        byte[] bytes0 = (byte[])list0[i];
                        byte[] bytes1 = (byte[])list1[i];

                        return bytes0.SequenceEqual(bytes1);
                    }
                    else if (!list0[i].Equals(list1[i])) return false;
                }
            }
        }

        return true;

    }




    public struct ProbabilityMinMaxCreateCount
    {
        public float prob;
        public int min; 
        public int max;

        public ProbabilityMinMaxCreateCount(float prob, int min, int max)
        {
            this.prob = prob; 
            this.min = min; 
            this.max = max;
        }

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
