using System;

namespace Optima.Fais.Dto
{
    public class DivisionSync
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int? DepartmentId { get; set; }
        public int? LocationId { get; set; }
        public int? RoomId { get; set; }
        public DateTime ModifiedAt { get; set; }

    }
}
