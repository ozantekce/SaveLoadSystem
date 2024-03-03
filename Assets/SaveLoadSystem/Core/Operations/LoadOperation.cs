using System;

namespace SaveLoadSystem.Core
{
    internal class LoadOperation : IOperation
    {
        public OperationStatus Status { get; set; }
        public bool RunAsync { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public SaveMode SaveStrategy { get; set; }
        public EncryptionType EncryptionType { get; set; }
        public string EncryptionKey { get; set; }
        public Action<SavableData> Callback { get; set; }

        public SavableData Result { get; set; }

        public string ID => $"{Path}_{FileName}_{SaveStrategy}";

        public void Start()
        {
            Status = OperationStatus.InProgress;

            ISaveLoadStrategy strategy = ISaveLoadStrategy.GetInstance(SaveStrategy);
            Result = strategy.Load(Path, FileName, RunAsync, EncryptionType, EncryptionKey);
            Callback?.Invoke(Result);

            if (RunAsync) Result = null; // clear

            Status = OperationStatus.Completed;
        }


    }

}
