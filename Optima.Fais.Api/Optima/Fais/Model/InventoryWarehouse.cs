using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
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