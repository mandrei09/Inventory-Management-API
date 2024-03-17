using System;

namespace Optima.Fais.Dto
{
    public class CostCenterSync
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int? DivisionId { get; set; }
        public int? RegionId { get; set; }
        public int? AdministrationId { get; set; }
        public int? AdmCenterId { get; set; }
        public int? LocationId { get; set; }
        public int? RoomId { get; set; }
        public int? IsFinished { get; set; }
        public DateTime ModifiedAt { get; set; }

    }
}
