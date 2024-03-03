namespace SaveLoadSystem.Core
{
    public interface ISavable
    {

        public void LoadSavedData(SavableData data);

        public SavableData ConvertToSavableData();

    }

}
