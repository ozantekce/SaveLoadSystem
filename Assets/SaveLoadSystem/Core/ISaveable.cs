using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SaveLoadSystem.Core
{

    public interface ISaveable
    {

        public void LoadSavedData(SaveableData data);


        public SaveableData CreateSaveData();




    }


}
