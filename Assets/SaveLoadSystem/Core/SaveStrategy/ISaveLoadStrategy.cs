namespace SaveLoadSystem.Core
{
    internal interface ISaveLoadStrategy
    {

        public string FileExtension { get; }

        public void Save(SaveableData saveableData, string path, string fileName, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "");

        public SaveableData Load(string path, string fileName, EncryptionType encryptionType = EncryptionType.None, string encryptionKey = "");

    }
}