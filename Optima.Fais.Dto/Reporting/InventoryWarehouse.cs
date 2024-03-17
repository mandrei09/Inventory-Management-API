using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryWarehouse
    {
        [Key]
        public int Id { get; set; }
        public decimal CurrentAPC { get; set; }
        public decimal CurrBkValue { get; set; }
        public decimal AccumulDep { get; set; }
        public int Initial { get; set; }
        public int Scanned { get; set; }
        public decimal Procentage { get; set; }
    }
}
