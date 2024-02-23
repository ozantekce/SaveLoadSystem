using System.Collections.Generic;
using System.Threading;

namespace SaveLoadSystem.Core
{
    internal class OperationsRunner
    {
        private const int MaxThreadCount = 16;
        private readonly object _lock = new object();

        private Dictionary<string, Queue<IOperation>> _idToQueuedOperations = new Dictionary<string, Queue<IOperation>>();
        private Dictionary<string, IOperation> _idToInProgressOperation = new Dictionary<string, IOperation>();

        public void Run()
        {
            List<string> removeIDs = new List<string>();
            while (true)
            {
                lock (_lock)
                {
                    removeIDs.Clear();
                    bool thereIsNoWaitingOperation = true;

                    int totalThreadCount = 0;
                    foreach (var operation in _idToInProgressOperation.Values)
                    {
                        if(operation.Status != OperationStatus.Completed)
                        {
                            totalThreadCount++;
                        }
                    }

                    if(totalThreadCount < MaxThreadCount)
                    {
                        foreach (var idQueuePair in _idToQueuedOperations)
                        {
                            string id = idQueuePair.Key;
                            Queue<IOperation> operations = idQueuePair.Value;

                            IOperation inProgressOperation = _idToInProgressOperation.GetValueOrDefault(id);
                            IOperation waitingOperation = null;
                            if (operations.Count != 0) waitingOperation = operations.Peek();

                            if (waitingOperation == null && inProgressOperation == null)
                            {
                                removeIDs.Add(id);
                            }
                            if(waitingOperation != null)
                            {
                                thereIsNoWaitingOperation = false;
                                if (totalThreadCount < MaxThreadCount && CanRunOperation(waitingOperation))
                                {
                                    StartOperation(waitingOperation);
                                    operations.Dequeue();
                                    totalThreadCount++;
                                }
                            }

                        }
                    }

                    foreach (string r_id in removeIDs)
                    {
                        _idToQueuedOperations.Remove(r_id);
                        _idToInProgressOperation.Remove(r_id);
                    }

                    if(thereIsNoWaitingOperation && totalThreadCount == 0)
                    {
                        //UnityEngine.Debug.Log("Exit");
                        _idToQueuedOperations.Clear();
                        _idToInProgressOperation.Clear();
                        return; // exit
                    }

                }

                Thread.Sleep(10); // delay
            }
        }

        public void AddOperation(IOperation operation)
        {
            lock (_lock)
            {
                GetOrAddQueue(operation.ID).Enqueue(operation);
            }
        }

        private bool CanRunOperation(IOperation operation)
        {
            return !_idToInProgressOperation.TryGetValue(operation.ID, out var inProgressOperation) || inProgressOperation.Status == OperationStatus.Completed;
        }

        private void StartOperation(IOperation operation)
        {
            var operationThread = new Thread(() =>
            {
                try
                {
                    operation.Start();
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.Log(ex);
                }
            });
            operationThread.Start();

            _idToInProgressOperation[operation.ID] = operation;
        }

        private Queue<IOperation> GetOrAddQueue(string id)
        {
            if (!_idToQueuedOperations.TryGetValue(id, out var queue))
            {
                queue = new Queue<IOperation>();
                _idToQueuedOperations[id] = queue;
            }
            return queue;
        }
    }
}
