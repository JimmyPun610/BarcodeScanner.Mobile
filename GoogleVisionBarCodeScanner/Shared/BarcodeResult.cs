namespace GoogleVisionBarCodeScanner
{
    public class BarcodeResult
    {
        public BarcodeTypes BarcodeType { get; set; }
        public BarcodeFormats BarcodeFormat { get; set; }
        public string DisplayValue { get; set; }
        public string RawValue { get; set; }
        public BarcodePoint[] CornerPoints { get; set; }
    }

    public struct BarcodePoint
    {
        public double X { get; }
        public double Y { get; }

        public BarcodePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"X:{X:0.00} Y:{Y:0.00}";
    }
}
