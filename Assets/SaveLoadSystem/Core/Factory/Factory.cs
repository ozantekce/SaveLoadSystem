using System;
using System.Collections.Generic;

namespace SaveLoadSystem.Core
{

    public static class Factory
    {

        private static Dictionary<Type, Func<SaveableData, ISaveable>> ConvertingStrategies = new Dictionary<Type, Func<SaveableData, ISaveable>>();



        public static T Create<T>(this SaveableData data) where T : ISaveable
        {
            if (data == null) return default;
            if (!ConvertingStrategies.ContainsKey(data.GetType())) return default;

            return (T)ConvertingStrategies[data.GetType()](data);
        }


        public static void AddConvertingStrategy(Type type, Func<SaveableData, ISaveable> strategy)
        {
            ConvertingStrategies[type] = strategy;
        }

        public static void RemoveConvertingStrategy(Type type)
        {
            ConvertingStrategies.Remove(type);
        }



    }

}
