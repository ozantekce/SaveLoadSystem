using System;

namespace SaveLoadSystem.Core
{
    internal class SaveOperation<T> : IOperation
    {
        public OperationStatus Status { get; set; }
        public bool RunAsync { get; set; }
        public T Data { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public SaveMode SaveStrategy { get; set; }
        public EncryptionType EncryptionType { get; set; }
        public string EncryptionKey { get; set; }
        public Action Callback { get; set; }

        public string ID => $"{Path}_{FileName}_{SaveStrategy}";

        public void Start()
        {
            if (Status != OperationStatus.NotStarted) return;

            Status = OperationStatus.InProgress;

            SavableData savableData = ConvertToSavableData(Data);
            ISaveLoadStrategy strategy = ISaveLoadStrategy.GetInstance(SaveStrategy);
            strategy.Save(savableData, Path, FileName, RunAsync, EncryptionType, EncryptionKey);
            
            Callback?.Invoke();

            Status = OperationStatus.Completed;
        }


        private SavableData ConvertToSavableData(T data)
        {
            return data is ISavable savable ? savable.ConvertToSavableData() : data as SavableData;
        }



    }

}
