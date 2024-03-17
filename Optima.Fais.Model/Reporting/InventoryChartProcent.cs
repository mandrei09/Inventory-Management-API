using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class InventoryChartProcentage
    {
        [Key]
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public int Initial { get; set; }
        public int Scanned { get; set; }
        public decimal Procentage { get; set; }
    }
}