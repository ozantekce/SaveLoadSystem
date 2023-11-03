using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{

    public void LoadSavedData(SaveableDataWrapper data);


    public SaveableDataWrapper CreateSaveData();




}
