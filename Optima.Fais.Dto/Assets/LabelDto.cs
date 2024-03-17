using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class LabelDto
    {
        public string InvNo { get; set; }
        public string Description { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string SapCode { get; set; }
        public string SerialNumber { get; set; }
    }
}
