using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;



namespace SaveLoadSystem
{
    public interface ISaveLoadStrategy
    {

        public string FileExtension { get; }

        public void Save(ISaveable saveable, string path, string fileName, bool encrypt = false, string encryptionKey = null);


        public SaveableData Load(string path, string fileName, bool decrypt = false, string decryptionKey = null);




    }


}