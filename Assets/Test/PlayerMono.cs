using SaveLoadSystem.Core;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMono : MonoBehaviour, ISaveable
{

    public ComponentMono component;



    public virtual SaveableData ConvertToSaveableData()
    {
        SaveableData saveableData = new SaveableData();

        saveableData.Write("transform", TransformToSaveableData());

        List<ISaveable> saveableComponents = new List<ISaveable>(GetComponents<ISaveable>());

        SaveableData components = new SaveableData();
        components.Write(nameof(components), saveableComponents);

        saveableData.Write("components", components);



        return saveableData;
    }

    public virtual void LoadSavedData(SaveableData data)
    {
        throw new System.NotImplementedException();
    }



    private SaveableData TransformToSaveableData()
    {
        SaveableData transformData = new SaveableData();

        transformData.Write("position", transform.localPosition);
        transformData.Write("rotation", transform.localRotation);
        transformData.Write("scale", transform.localScale);

        return transformData; 
    }



}
