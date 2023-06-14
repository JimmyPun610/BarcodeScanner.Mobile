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

        public IList<OCRLine> Lines { get; set; } = new List<OCRLine>();

        public IList<string> GetAllElements()
        {
            List<string> AllElements = new List<string>();
            foreach (var line in Lines)
                AllElements.AddRange(line.Elements);
            
            return AllElements;
        }

        public class OCRLine
        {
            public string Text { get; set; }
            public IList<string> Elements { get; set; } = new List<string>();
        }

    }
}
