namespace QR_Code_Generator
{
    /// <summary>
    /// This enum represents the correction level of QR-code.
    /// It shows the percentage of maximum possible data loss.
    /// </summary>
    internal enum CorrectionLevel
    {
        L, // 7%
        M, // 15%
        Q, // 25%
        H  // 30%
    }
}
