using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class AuditInventoryResult
    {
        public string Region { get; set; }
        public string Location { get; set; }
        public string LocationCode { get; set; }
        public int Items { get; set; }
        public int NotScanned { get; set; }
        public int Minus { get; set; }
        public int Plus { get; set; }
        // public int Temporary { get; set; }
        // public int TranInLocation { get; set; }
        // public int TranBetweenLocations { get; set; }
        // public int TranInAdmCenter { get; set; }
        // public int TranBetweenAdmCenters { get; set; }
        // public string Route { get; set; }
        public string Member1 { get; set; }
        public string Member2 { get; set; }
        public string Member3 { get; set; }
        public string Member4 { get; set; }
        public string Member5 { get; set; }
        public string Member6 { get; set; }
        public string Member7 { get; set; }
        public string Employee { get; set; }
        public string Employee2 { get; set; }
        public string Employee3 { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string InventoryName { get; set; }
        public DateTime InventoryEndDate { get; set; }
        public DateTime InventoryStartDate { get; set; }
        public decimal ValueMinus { get; set; }
        public decimal ValueNotScanned { get; set; }
        // public int Reco { get; set; }
        public int Cassation { get; set; }
        // public DateTime MinDateAdmCenter { get; set; }
        // public DateTime MaxDateAdmCenter { get; set; }
         public DateTime MinDateLocation { get; set; }
         public DateTime MaxDateLocation { get; set; }
        // public int DiffAdmCenter { get; set; }
        // public int DiffLocation { get; set; }

        public List<AuditInventoryResultDetail> Details;

        public List<AuditInventoryResultDetail> ListInventoryResultDetail()
        {
            return Details;
        }
    }
}
