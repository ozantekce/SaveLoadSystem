using SaveLoadSystem.Core;
using UnityEngine;

public class SubGameObjectMono : MonoBehaviour, ISaveable
{

    public int value;

    public SaveableData ConvertToSaveableData()
    {
        SaveableData saveableData = new SaveableData();
        saveableData.Write(nameof(value), value);
        return saveableData;
    }

    public void LoadSavedData(SaveableData data)
    {
        value = data.ReadOrDefault(nameof(value), value);
    }
}
