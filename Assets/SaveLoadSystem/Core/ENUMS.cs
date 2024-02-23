
public enum SaveMode
{
    CustomSerialize = 0,
    Json = 1,
    Serialize = 2,

}

public enum EncryptionType
{
    None = 0,
    XOR = 1,
    AES = 2,
    CaesarCipher = 3,
    
}


public enum OperationStatus
{
    NotStarted,
    InProgress,
    Completed
}