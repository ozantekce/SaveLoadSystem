using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;



namespace SaveLoadSystem
{
    internal interface ISaveLoadStrategy
    {

        public string FileExtension { get; }

        public void Save(SaveableData saveableData, string path, string fileName, bool encrypt = false, string encryptionKey = null);


        public SaveableData Load(string path, string fileName, bool decrypt = false, string decryptionKey = null);




    }


}