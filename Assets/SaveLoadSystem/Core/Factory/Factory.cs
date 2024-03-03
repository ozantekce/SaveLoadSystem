using System;
using System.Collections.Generic;

namespace SaveLoadSystem.Core
{

    public static class Factory
    {

        private static Dictionary<Type, Func<SavableData, ISavable>> ConvertingStrategies = new Dictionary<Type, Func<SavableData, ISavable>>();



        public static T Create<T>(this SavableData data) where T : ISavable
        {
            if (data == null) return default;
            if (!ConvertingStrategies.ContainsKey(data.GetType())) return default;

            return (T)ConvertingStrategies[data.GetType()](data);
        }


        public static void AddConvertingStrategy(Type type, Func<SavableData, ISavable> strategy)
        {
            ConvertingStrategies[type] = strategy;
        }

        public static void RemoveConvertingStrategy(Type type)
        {
            ConvertingStrategies.Remove(type);
        }



    }

}
