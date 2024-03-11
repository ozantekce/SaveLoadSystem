using SaveLoadSystem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{

    public SaveMode saveMode;
    public EncryptionType encryptionType;
    public string fileName = "SaveSlot1";
    public int randomTestSize = 100;



    [ContextMenu("Save")]
    public void Save()
    {

        SavableData saveData = new SavableData();

        saveData.Write("longValue", 9999999L);
        saveData.Write("doubleValue", 9999999.88D);
        saveData.Write("color", new Color(0, 50, 0));
        saveData.Write("q", transform.rotation);
        saveData.Write("dateTime", System.DateTime.Now);

        saveData.Write("vector2", new Vector2(10, 20));

        saveData.Write<string>("nullTest", null);

        List<int> array = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            array.Add(i);
        }

        List<string> strList = new List<string> { "a", "b", null, "c" };

        saveData.Write("list", array);
        saveData.Write("strList", strList);
        SaveLoadManager.Save(saveData, fileName, saveMode);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        SavableData loadedData = SaveLoadManager.Load("SaveSlot1", saveMode);
        Debug.Log(loadedData.Read<long>("longValue"));
        Debug.Log(loadedData.Read<double>("doubleValue"));
        Debug.Log(loadedData.Read<Color>("color"));
        Debug.Log(loadedData.Read<Vector2>("vector2"));
        Debug.Log(loadedData.Read<Quaternion>("q"));
        Debug.Log(loadedData.Read<DateTime>("dateTime"));

        List<int> array = loadedData.Read<List<int>>("list");

        Debug.Log(loadedData.Read<string>("nullTest") == null);

        foreach (var item in array)
        {
            Debug.Log("array : " + item);
        }

        List<string> strList = loadedData.Read<List<string>>("strList");

        foreach (var item in strList)
        {
            Debug.Log("strList : " + item);
        }

    }


    [ContextMenu("Test")]
    void TestMethod()
    {
        // Create a new SavableData instance and write various types of data
        SavableData saveData = new SavableData();

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


        SavableData deserializedSaveData = SaveLoadManager.Load(fileName, saveMode);

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
    void TestComplexSavableData()
    {
        // Root SavableData
        SavableData rootSaveData = new SavableData();
        rootSaveData.Write("rootLong", 123456789L);
        rootSaveData.Write("rootString", "RootLevelData");

        // Nested SavableData
        SavableData nestedSaveData = new SavableData();
        nestedSaveData.Write("nestedInt", 987);
        nestedSaveData.Write("nestedBool", true);

        // Adding the nested SavableData to the root
        rootSaveData.Write("nestedData", nestedSaveData);

        // Creating a list of SavableData for complexity
        List<SavableData> listOfSavableData = new List<SavableData>();
        for (int i = 0; i < 3; i++)
        {
            SavableData listSavableData = new SavableData();
            listSavableData.Write("index", i);
            listSavableData.Write("nestedString", $"Item{i}");
            listOfSavableData.Add(listSavableData);
        }

        // Writing the list of SavableData to the root
        rootSaveData.Write("listOfNestedData", listOfSavableData);

        // Sub-nested SavableData for even more complexity
        SavableData subNestedSaveData = new SavableData();
        subNestedSaveData.Write("subNestedFloat", 3.14f);

        // Adding a sub-nested SavableData to one of the list items
        listOfSavableData[1].Write("subNestedData", subNestedSaveData);

        SaveLoadManager.Save(rootSaveData, fileName, saveMode);


        SavableData deserializedRootSaveData = SaveLoadManager.Load(fileName, saveMode);

        // Verification: Check that the root level data matches
        Debug.Assert(rootSaveData.Read<long>("rootLong") == deserializedRootSaveData.Read<long>("rootLong"), "Root level long value mismatch");
        Debug.Assert(rootSaveData.Read<string>("rootString") == deserializedRootSaveData.Read<string>("rootString"), "Root level string value mismatch");

        // Verification: Check that the nested SavableData is correct
        SavableData deserializedNestedData = deserializedRootSaveData.Read<SavableData>("nestedData");
        Debug.Assert(nestedSaveData.Read<int>("nestedInt") == deserializedNestedData.Read<int>("nestedInt"), "Nested int value mismatch");
        Debug.Assert(nestedSaveData.Read<bool>("nestedBool") == deserializedNestedData.Read<bool>("nestedBool"), "Nested bool value mismatch");

        // Verification: Check that the list of SavableData is correct
        List<SavableData> deserializedListOfSavableData = deserializedRootSaveData.Read<List<SavableData>>("listOfNestedData");
        for (int i = 0; i < listOfSavableData.Count; i++)
        {
            Debug.Assert(listOfSavableData[i].Read<int>("index") == deserializedListOfSavableData[i].Read<int>("index"), $"List item {i} index value mismatch");
            Debug.Assert(listOfSavableData[i].Read<string>("nestedString") == deserializedListOfSavableData[i].Read<string>("nestedString"), $"List item {i} string value mismatch");
        }

        // Verification: Check the sub-nested SavableData
        SavableData deserializedSubNestedData = deserializedListOfSavableData[1].Read<SavableData>("subNestedData");
        Debug.Assert(subNestedSaveData.Read<float>("subNestedFloat") == deserializedSubNestedData.Read<float>("subNestedFloat"), "Sub-nested float value mismatch");

        // Output result
        Debug.Log("All complex tests passed!");
    }


    [ContextMenu("Test2")]
    void TestComplex2SavableData()
    {
        // Root SavableData
        SavableData rootSaveData = new SavableData();
        SavableData f0 = new SavableData();
        SavableData f1 = new SavableData();
        SavableData f0_0 = new SavableData();
        SavableData f0_0_1 = new SavableData();
        SavableData f1_0 = new SavableData();
        SavableData f1_1 = new SavableData();
        SavableData f1_2 = new SavableData();

        List<SavableData> list = new List<SavableData>();
        SavableData listNode0 = new SavableData();
        SavableData listNode1 = new SavableData();
        SavableData listNode2 = new SavableData();

        SavableData listNode1_0 = new SavableData();

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


        SavableData deserializedRootSaveData = SaveLoadManager.Load(fileName, saveMode);

        // Begin testing assertions
        Debug.Assert(deserializedRootSaveData.Read<SavableData>("f0").Read<SavableData>("f0").Read<int>("f0") == 20, "Nested int value mismatch");
        Debug.Assert(deserializedRootSaveData.Read<SavableData>("f0").Read<SavableData>("f0").Read<SavableData>("f1").Read<List<SavableData>>("f0").Count == list.Count, "List<SavableData> count mismatch");

        // Test the list of SavableData
        SavableData deserializedListNode0 = deserializedRootSaveData.Read<SavableData>("f0").Read<SavableData>("f0").Read<SavableData>("f1").Read<List<SavableData>>("f0")[0];
        SavableData deserializedListNode1 = deserializedRootSaveData.Read<SavableData>("f0").Read<SavableData>("f0").Read<SavableData>("f1").Read<List<SavableData>>("f0")[1];
        SavableData deserializedListNode2 = deserializedRootSaveData.Read<SavableData>("f0").Read<SavableData>("f0").Read<SavableData>("f1").Read<List<SavableData>>("f0")[2];

        Debug.Assert(deserializedListNode0.Read<int>("f0") == 10, "List node 0 value mismatch");
        Debug.Assert(deserializedListNode1.Read<SavableData>("f0").Read<int>("f0").Equals(listNode1_0.Read<int>("f0")), "List node 1 SavableData mismatch");
        Debug.Assert(deserializedListNode2.Read<int>("f0") == 25, "List node 2 value mismatch");

        // Test the direct children of root
        Debug.Assert(deserializedRootSaveData.Read<SavableData>("f1").Read<SavableData>("f0").Read<int>("f0") == 5, "Direct child f1_0 value mismatch");
        Debug.Assert(deserializedRootSaveData.Read<SavableData>("f1").Read<SavableData>("f1").Read<int>("f1") == 30, "Direct child f1_1 value mismatch");
        Debug.Assert(deserializedRootSaveData.Read<SavableData>("f1").Read<SavableData>("f2").Read<string>("f2").Equals("Hi"), "Direct child f1_2 value mismatch");

        // Output result
        Debug.Log("All complex structure tests passed!");



    }


    [ContextMenu("RandomTest")]
    public void RandomTest()
    {

        int totalCreated = 0;
        SavableData root = CreateRandomSavableData(randomTestSize, ref totalCreated);

        Debug.Log("total test data size : " + totalCreated);

        SavableData deserializedSaveData;
        bool testResult;
        Stopwatch stopwatch = new Stopwatch();
        DateTime startTime, endTime;
        TimeSpan duration;

        //-------------------------------------------------------------------------
        // Custom Serialization
        stopwatch.Start();
        startTime = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.CustomSerialize, encryptionType);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Custom Serialization Saving takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");
        
        stopwatch.Restart();
        startTime = DateTime.Now;
        deserializedSaveData = SaveLoadManager.Load(fileName, SaveMode.CustomSerialize, encryptionType);
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
        SaveLoadManager.Save(root, fileName, SaveMode.Serialize, encryptionType);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Binary Saving takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        stopwatch.Restart();
        startTime = DateTime.Now;
        deserializedSaveData = SaveLoadManager.Load(fileName, SaveMode.Serialize, encryptionType);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"Binary Loading takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        testResult = IsSame(root, deserializedSaveData);
        Debug.Log($"Binary test result: {testResult}");
        //-------------------------------------------------------------------------
        Debug.Log("^#################################################################");

        
        // JSON Serialization
        stopwatch.Restart();
        startTime = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.Json, encryptionType);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"JSON Saving takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        stopwatch.Restart();
        startTime = DateTime.Now;
        deserializedSaveData = SaveLoadManager.Load(fileName, SaveMode.Json, encryptionType);
        endTime = DateTime.Now;
        duration = endTime - startTime;
        stopwatch.Stop();
        Debug.Log($"JSON Loading takes: {stopwatch.ElapsedMilliseconds} ms (Stopwatch) | {duration.TotalMilliseconds} ms (DateTime)");

        testResult = IsSame(root, deserializedSaveData);
        Debug.Log($"JSON test result: {testResult}");
        //-------------------------------------------------------------------------
        Debug.Log("^#################################################################");
        

    }

    [ContextMenu("RandomTestAsyncMultiple")]
    public void RandomTestAsyncMultiple()
    {

        int totalCreated = 0;
        SavableData root = CreateRandomSavableData(randomTestSize, ref totalCreated);

        Debug.Log("total test data size : " + totalCreated);
        for (int i = 0; i < 100; i++)
        {
            RandomTestAsync($"SaveSlot{i}", root);
        }

    }


    public void RandomTestAsync(string fileName, SavableData root)
    {


        Stopwatch[] stopwatchs = new Stopwatch[6];
        DateTime[] startTimes = new DateTime[6];
        DateTime[] endTimes = new DateTime[6];
        TimeSpan[] durations = new TimeSpan[6];

        //-------------------------------------------------------------------------
        // Custom Serialization
        stopwatchs[0] = new Stopwatch();
        stopwatchs[0].Start();
        startTimes[0] = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.CustomSerialize, encryptionType, true, () =>
        {
            endTimes[0] = DateTime.Now;
            durations[0] = endTimes[0] - startTimes[0];
            stopwatchs[0].Stop();
            Debug.Log($"Custom Serialization Saving takes: {stopwatchs[0].ElapsedMilliseconds} ms (Stopwatch) | {durations[0].TotalMilliseconds} ms (DateTime)");
        });

        stopwatchs[1] = new Stopwatch();
        stopwatchs[1].Start();
        startTimes[1] = DateTime.Now;
        SaveLoadManager.Load(fileName, SaveMode.CustomSerialize, encryptionType, true, deserializedSaveData =>
        {
            endTimes[1] = DateTime.Now;
            durations[1] = endTimes[1] - startTimes[1];
            stopwatchs[1].Stop();
            Debug.Log($"Custom Serialization Loading takes: {stopwatchs[1].ElapsedMilliseconds}  ms (Stopwatch) |  {durations[1].TotalMilliseconds} ms (DateTime)");

            bool testResult = IsSame(root, deserializedSaveData);
            Debug.Log($"Custom Serialization test result: {testResult}");
        });

        /*

        //-------------------------------------------------------------------------
        // Binary Serialization
        stopwatchs[2] = new Stopwatch();
        stopwatchs[2].Start();
        startTimes[2] = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.Serialize, encryptionType, true, () =>
        {
            endTimes[2] = DateTime.Now;
            durations[2] = endTimes[2] - startTimes[2];
            stopwatchs[2].Stop();
            Debug.Log($"Binary Serialization Saving takes: {stopwatchs[2].ElapsedMilliseconds}   ms (Stopwatch) |   {durations[2].TotalMilliseconds} ms (DateTime)");

        });

        stopwatchs[3] = new Stopwatch();
        stopwatchs[3].Start();
        startTimes[3] = DateTime.Now;
        SaveLoadManager.Load(fileName, SaveMode.Serialize, encryptionType, true, deserializedSaveData =>
        {
            endTimes[3] = DateTime.Now;
            durations[3] = endTimes[3] - startTimes[3];
            stopwatchs[3].Stop();
            Debug.Log($"Binary Serialization Loading takes: {stopwatchs[3].ElapsedMilliseconds} ms (Stopwatch) | {durations[3].TotalMilliseconds} ms (DateTime)");

            bool testResult = IsSame(root, deserializedSaveData);
            Debug.Log($"Binary Serialization test result: {testResult}");
        });

        */


        /*
        //-------------------------------------------------------------------------
        // Json Serialization
        stopwatchs[4] = new Stopwatch();
        stopwatchs[4].Start();
        startTimes[4] = DateTime.Now;
        SaveLoadManager.Save(root, fileName, SaveMode.Json, encryptionType, true, () =>
        {
            endTimes[4] = DateTime.Now;
            durations[4] = endTimes[4] - startTimes[4];
            stopwatchs[4].Stop();
            Debug.Log($"Json Serialization Saving takes: {stopwatchs[4].ElapsedMilliseconds}   ms (Stopwatch) |   {durations[4].TotalMilliseconds} ms (DateTime)");

        });

        stopwatchs[5] = new Stopwatch();
        stopwatchs[5].Start();
        startTimes[5] = DateTime.Now;
        SaveLoadManager.Load(fileName, SaveMode.Json, encryptionType, true, deserializedSaveData =>
        {
            endTimes[5] = DateTime.Now;
            durations[5] = endTimes[5] - startTimes[5];
            stopwatchs[5].Stop();
            Debug.Log($"Json Serialization Loading takes: {stopwatchs[5].ElapsedMilliseconds} ms (Stopwatch) | {durations[5].TotalMilliseconds} ms (DateTime)");

            bool testResult = IsSame(root, deserializedSaveData);
            Debug.Log($"Json Serialization test result: {testResult}");
        });
        */


    }




    private SavableData CreateRandomSavableData(int minCount, ref int totalCreated)
    {

        Dictionary<DataType, ProbabilityMinMaxCreateCount> dataProbabilities = new Dictionary<DataType, ProbabilityMinMaxCreateCount>
        {
            { DataType.Int, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Float, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Long, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Double, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.String, new ProbabilityMinMaxCreateCount(0.3f, 3, 20) },
            { DataType.Vector3, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Vector2, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Color, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.Quaternion, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.DateTime, new ProbabilityMinMaxCreateCount(0.5f, 3, 20) },
            { DataType.SavableData, new ProbabilityMinMaxCreateCount(0.3f, 1, 2) },
            { DataType.List_Int, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Float, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Long, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Double, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Bool, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_String, new ProbabilityMinMaxCreateCount(0.05f, 1, 5) },
            { DataType.List_Vector3, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Vector2, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Color, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_Quaternion, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_DateTime, new ProbabilityMinMaxCreateCount(0.1f, 1, 5) },
            { DataType.List_SavableData, new ProbabilityMinMaxCreateCount(0.02f, 1, 1) },
            { DataType.Null, new ProbabilityMinMaxCreateCount(0.5f, 3, 10) }
        };


        string fieldName = "f";

        SavableData root = new SavableData();
        int currentCount = 0;
        Queue<SavableData> frontier = new Queue<SavableData>();

        frontier.Enqueue(root);

    Continue:
        while (frontier.Count > 0 && currentCount < minCount)
        {

            SavableData current = frontier.Dequeue();

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
                            if (dt == DataType.SavableData)
                            {
                                SavableData child = new SavableData();
                                frontier.Enqueue(child);
                                current.Write(fieldName + currentCount, child);
                                currentCount++;
                                totalCreated++;
                            }
                            else if(dt == DataType.List_SavableData)
                            {
                                List<SavableData> list = new List<SavableData>();
                                for (int k = 0; k < 5; k++)
                                {
                                    list.Add(CreateRandomSavableData(5,ref totalCreated));
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


    public bool IsSame(SavableData s0, SavableData s1)
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
        else if(d0.Type == DataType.SavableData)
        {
            return IsSame(d0.GetValue<SavableData>(), d1.GetValue<SavableData>());
        }
        else if(d0.Type == DataType.Null && d1.Type == DataType.Null)
        {
            return true;
        }
        else
        {
            IList list0 = (IList)d0.Value;
            IList list1 = (IList)d1.Value;
            if(list0.Count != list1.Count) return false;
            for (int i = 0; i < list0.Count; i++)
            {
                if (d0.Type == DataType.List_SavableData)
                {
                    if (!IsSame((SavableData)list0[i], (SavableData)list1[i])) return false;
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






}
