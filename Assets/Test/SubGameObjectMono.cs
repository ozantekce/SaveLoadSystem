using SaveLoadSystem.Core;
using UnityEngine;

public class SubGameObjectMono : MonoBehaviour, ISavable
{

    public int value;

    public SavableData ConvertToSavableData()
    {
        SavableData savableData = new SavableData();
        savableData.Write(nameof(value), value);
        return savableData;
    }

    public void LoadSavedData(SavableData data)
    {
        value = data.ReadOrDefault(nameof(value), value);
    }
}
