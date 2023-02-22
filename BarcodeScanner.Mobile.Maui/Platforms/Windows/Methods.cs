namespace BarcodeScanner.Mobile
{
    // All the code in this file is only included on Windows.
    public class Methods
    {
        /// <summary>
        /// There is no support on Windows
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> AskForRequiredPermission()
        {
            return false;
        }
    }
}