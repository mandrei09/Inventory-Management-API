using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class InternalEntryNoteAsset
    {
        public string Name { get; set; }
        public string Destination { get; set; }
        public string MeasureUnit { get; set; }
        public string SourceDocumentQuantity { get; set; }
        public string Quantity { get; set; }
        public float PricePerUnit { get; set; }
        public float Value { get; set; }
    }
}
