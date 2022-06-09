namespace BarcodeScanner.Mobile.Core
{
    public class BarcodeResult
    {
        public BarcodeTypes BarcodeType { get; set; }
        public BarcodeFormats BarcodeFormat { get; set; }
        public string DisplayValue { get; set; }
        public string RawValue { get; set; }
        /// <summary>
        /// This value is native coordination, please make conversion to Maui coordination first
        /// </summary>
        public Microsoft.Maui.Graphics.Rect BoundingBox { get; set; }
    }
    
}
