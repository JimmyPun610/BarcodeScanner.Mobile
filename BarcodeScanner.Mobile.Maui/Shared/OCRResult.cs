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

        public IList<Line> Lines { get; set; }

        public IList<string> GetAllElements()
        {
            return null;
        }

        public class Line
        {
            public string Text { get; set; }
            public IList<string> Elements { get; set; } = new List<string>();
        }

    }
}
