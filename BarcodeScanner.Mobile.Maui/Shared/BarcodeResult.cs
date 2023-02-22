
namespace BarcodeScanner.Mobile
{
    public class BarcodeResult
    {
        public BarcodeTypes BarcodeType { get; set; }
        public BarcodeFormats BarcodeFormat { get; set; }
        public string DisplayValue { get; set; }
        public string RawValue { get; set; }
        /// <summary>
        /// This value is native coordination, please be reminded that you may not able to use it directly...
        /// </summary>
        public Point[] CornerPoints { get; set; }

        public byte[] RawData { get; set; }
    }
    
}
