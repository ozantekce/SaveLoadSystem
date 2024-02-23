
namespace SaveLoadSystem.Core
{
    internal interface IOperation
    {
        string ID { get; }
        OperationStatus Status { get; }
        bool RunAsync { get; }
        string Path { get; }
        string FileName { get; }
        SaveMode SaveStrategy { get; }
        EncryptionType EncryptionType { get; }
        string EncryptionKey { get; }

        void Start();

    }



}
