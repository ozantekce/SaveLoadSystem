using System;
using System.Threading;
using UnityEngine;

namespace SaveLoadSystem.Core
{
    public static class SaveLoadManager
    {
        private const string EncryptionKey = "5ZaX8nC2pY7kF4rO9gE0bL3tU1mQ6sWv";
        private static OperationsRunner OperationsRunner = new OperationsRunner();
        private static Thread MasterThread;

        public static void Save<T>(T data, string fileName, SaveMode saveStrategy = SaveMode.CustomSerialize, EncryptionType encryptionType = EncryptionType.None, bool runAsync = false, Action callback = null)
        {
            Save(data, null, fileName, saveStrategy, encryptionType, runAsync, callback);
        }

        public static SavableData Load(string fileName, SaveMode saveStrategy = SaveMode.CustomSerialize, EncryptionType encryptionType = EncryptionType.None, bool runAsync = false, Action<SavableData> callback = null)
        {
            return Load(null, fileName, saveStrategy, encryptionType, runAsync, callback);
        }


        public static void Save<T>(T data, string path, string fileName, SaveMode saveStrategy = SaveMode.CustomSerialize, EncryptionType encryptionType = EncryptionType.None, bool runAsync = false, Action callback = null)
        {

            string finalPath = string.IsNullOrEmpty(path) ? Application.persistentDataPath : path;
            SaveOperation<T> saveOperation = new SaveOperation<T>
            {
                RunAsync = runAsync,
                Data = data,
                Path = finalPath,
                FileName = fileName,
                SaveStrategy = saveStrategy,
                EncryptionType = encryptionType,
                EncryptionKey = EncryptionKey,
                Callback = callback
            };

            if (!runAsync)
            {
                ExecuteOperationSynchronously(saveOperation);
            }
            else
            {
                StartOperationAsync(saveOperation);
            }
        }

        public static SavableData Load(string path, string fileName, SaveMode saveStrategy = SaveMode.CustomSerialize, EncryptionType encryptionType = EncryptionType.None, bool runAsync = false, Action<SavableData> callback = null)
        {
            string finalPath = string.IsNullOrEmpty(path) ? Application.persistentDataPath : path;
            LoadOperation loadOperation = new LoadOperation
            {
                RunAsync = runAsync,
                Path = finalPath,
                FileName = fileName,
                SaveStrategy = saveStrategy,
                EncryptionType = encryptionType,
                EncryptionKey = EncryptionKey,
                Callback = callback
            };

            if (!runAsync)
            {
                ExecuteOperationSynchronously(loadOperation);
                return loadOperation.Result;
            }
            else
            {
                StartOperationAsync(loadOperation);
                return null;
            }
        }

        private static void ExecuteOperationSynchronously(IOperation operation)
        {
            try
            {
                operation.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Operation failed: {ex.Message}");
            }
        }

        private static void StartOperationAsync(IOperation operation)
        {
            OperationsRunner.AddOperation(operation);
            if (MasterThread == null || !MasterThread.IsAlive)
            {
                //Debug.Log("Create Master Thread");
                MasterThread = new Thread(OperationsRunner.Run) { IsBackground = true };
                MasterThread.Start();
            }
        }


    }
}
