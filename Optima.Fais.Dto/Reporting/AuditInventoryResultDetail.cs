using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class AuditInventoryResultDetail
    {
        public string Region { get; set; }
        public string Location { get; set; }
        public int Items { get; set; }
        public int NotScanned { get; set; }
        public int Minus { get; set; }
        public int Plus { get; set; }
        public int Temporary { get; set; }
        public int TranInLocation { get; set; }
        public int TranBetweenLocations { get; set; }
        public int TranInAdmCenter { get; set; }
        public int TranBetweenAdmCenters { get; set; }
        public string Route { get; set; }
        public string Member1 { get; set; }
        public string Member2 { get; set; }
        public string Member3 { get; set; }
        public string Member4 { get; set; }
        public string Member5 { get; set; }
        public string Member6 { get; set; }
        public string Member7 { get; set; }
        public string Member8 { get; set; }
        public string Member9 { get; set; }
        public string Member10 { get; set; }
        public string InventoryName { get; set; }
        public DateTime InventoryEndDate { get; set; }
        public decimal ValueMinus { get; set; }
        public decimal ValueNotScanned { get; set; }
        public int DiffAdmCenter { get; set; }
        public int DiffLocation { get; set; }
    }
}
