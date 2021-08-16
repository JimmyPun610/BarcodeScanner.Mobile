namespace GoogleVisionBarCodeScanner
{
    public class BarcodeResult
    {
        public BarcodeTypes BarcodeType { get; set; }
        public string DisplayValue { get; set; }
        public string RawValue { get; set; }
    }
}
