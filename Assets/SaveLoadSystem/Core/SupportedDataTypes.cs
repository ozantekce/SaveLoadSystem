
namespace SaveLoadSystem.Core
{

    public enum DataType : byte
    {
        // Basic Types
        Int = 0,
        Float = 1,
        Long = 2,
        Double = 3,
        Bool = 4,
        String = 5,
        //

        // Others
        Vector3 = 6,
        Vector2 = 7,
        Color = 8,
        Quaternion = 9,
        DateTime = 10,

        SavableData = 11,
        //

        // Lists
        List_Int = 12,
        List_Float = 13,
        List_Long = 14,
        List_Double = 15,
        List_Bool = 16,
        List_String = 17,

        List_Vector3 = 18,
        List_Vector2 = 19,
        List_Color = 20,
        List_Quaternion = 21,
        List_DateTime = 22,

        List_SavableData = 23,
        //

        Null = 24,

    }

}