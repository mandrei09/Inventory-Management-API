using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class ImportDemo
    {
        public string Uom { get; set; }
        public string InvNo { get; set; }
        public string Description { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public string CostCenter { get; set; }
        public decimal ValueRem { get; set; }
        public DateTime? PIF { get; set; }
        public int MonthRem { get; set; }
        public string AppState { get; set; }
        public bool AllowLabel { get; set; }
        public string SerialNumber { get; set; }
        public string AssetCategory { get; set; }
        public string EmployeeFull { get; set; }
        public string InternalCode { get; set; }
        public string Region { get; set; }
        public string LocationCode { get; set; }
        public string LocationType { get; set; }
        public string LocationName { get; set; }
        public string Room { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
      
    }
}
