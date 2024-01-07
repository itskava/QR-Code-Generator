namespace QR_Code_Generator
{
    /// <summary>
    /// The enum that represents the chosen encoding type.
    /// Numeric type allows to encode only digits, alphanumeric type allows to encode uppercase
    /// letters, digits and some symbols ('$', '%', '*', '+', '-', '.', '/', ':' and space)
    /// and binary type allows to encode any characters (this generator uses utf-8).
    /// </summary>
    internal enum EncodingMethod
    {
        Numeric,
        Alphanumeric,
        Binary
    }
}
