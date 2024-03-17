using System;

namespace Optima.Fais.Dto
{
    public class ImportModel
    {
		public string DictionaryItem { get; set; }
		public string Brand { get; set; }
		public string Model { get; set; }
		public int SNLength { get; set; }
		public int IMEILength { get; set; }
        public string Active { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
    }
}
