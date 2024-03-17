using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class InventoryTotal
    {
        [Key]
        public int Id { get; set; }
        public decimal CurrentAPC { get; set; }
        public decimal CurrBkValue { get; set; }
        public decimal AccumulDep { get; set; }
        public int Initial { get; set; }
        public int Scanned { get; set; }
        public decimal ScannedCurrentAPC { get; set; }
        public decimal ScannedCurrBkValue { get; set; }
        public decimal ScannedAccumulDep { get; set; }
        public decimal Procentage { get; set; }
    }
}
