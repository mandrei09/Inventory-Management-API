using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class AuditInventoryV1T1
    {
        [Key]
        public int Id { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public int Initial { get; set; }
        public int Items { get; set; }
        public int NotScanned { get; set; }
        public int Minus { get; set; }
        public int Plus { get; set; }
        public int Temporary { get; set; }
        public int TranInLocation { get; set; }
        public int TranBetweenLocations { get; set; }
        public int TranInAdmCenter { get; set; }
        public int TranBetweenAdmCenters { get; set; }
    }
}
