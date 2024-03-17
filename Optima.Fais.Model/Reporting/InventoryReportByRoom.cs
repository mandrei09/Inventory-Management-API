using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class InventoryReportByRoom
    {
        [Key]
        public int Id { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string AdministrationCode { get; set; }
        public string AdministrationName { get; set; }
        public int Total { get; set; }
        public int Scanned { get; set; }
        public int AdministrationTransfer { get; set; }
        public decimal Procentage { get; set; }
    }
}
