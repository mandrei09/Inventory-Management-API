using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
   public class InventoryReportByAdmCenter
    {
        [Key]
        public int Id { get; set; }
        public string AdmCenterName { get; set; }
        public string AdmCenterCode { get; set; }
        public int Total { get; set; }
        public int Scanned { get; set; }
        public decimal Procentage { get; set; }
    }
}
