using SaveLoadSystem.Core;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMono : MonoBehaviour, ISavable
{

    public ComponentMono component;



    public virtual SavableData ConvertToSavableData()
    {
        SavableData savableData = new SavableData();

        savableData.Write("transform", TransformToSavableData());

        List<ISavable> savableComponents = new List<ISavable>(GetComponents<ISavable>());

        SavableData components = new SavableData();
        components.Write(nameof(components), savableComponents);

        savableData.Write("components", components);



        return savableData;
    }

    public virtual void LoadSavedData(SavableData data)
    {
        throw new System.NotImplementedException();
    }



    private SavableData TransformToSavableData()
    {
        SavableData transformData = new SavableData();

        transformData.Write("position", transform.localPosition);
        transformData.Write("rotation", transform.localRotation);
        transformData.Write("scale", transform.localScale);

        return transformData; 
    }



}
