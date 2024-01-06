namespace QR_Code_Generator
{
    /// <summary>
    /// The enum that represents the chosen encoding type.
    /// Digital type allows to encode only digits, alphanumeric type
    /// allows to encode uppercase letters, digits and some symbols 
    /// ('$', '%', '*', '+', '-', '.', '/', ':' and space) and byte type
    /// allows to encode any utf-8 characters.
    /// </summary>
    internal enum EncodingType
    {
        Digital,
        Alphanumeric,
        Byte
    }
}
