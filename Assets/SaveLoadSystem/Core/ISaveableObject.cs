namespace SaveLoadSystem.Core
{
    public interface ISaveableObject : ISaveable
    {
        public long ObjectID { get; }

    }

}
