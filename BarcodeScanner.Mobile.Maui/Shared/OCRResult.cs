using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScanner.Mobile
{
    public class OCRResult
    {
        public bool Success { get; set; }

        public string AllText { get; set; }

        public IList<OCRElement> Elements { get; set; } = new List<OCRElement>();

        public class OCRElement
        {
            public string Text { get; set; }
            public float Confidence { get; set; }
        }

    }
}
