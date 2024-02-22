namespace SaveLoadSystem.Core
{

    public interface ISaveable
    {

        public void LoadSavedData(SaveableData data);


        public SaveableData ConvertToSaveableData();


    }


}
