namespace GoogleVisionBarCodeScanner
{
    public class BarcodeResult
    {
        public BarcodeTypes BarcodeType { get; set; }
        public BarcodeFormats BarcodeFormat { get; set; }
        public string DisplayValue { get; set; }
        public string RawValue { get; set; }
        /// <summary>
        /// This value is native coordination, please make conversion to Xamarin Forms coordination first
        /// </summary>
        public System.Drawing.RectangleF BoundingBox { get; set; }
    }
    
}
