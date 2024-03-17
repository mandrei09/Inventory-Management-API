using System;

namespace Optima.Fais.Dto
{
    public class CitySync
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int? CountyId { get; set; }
        public DateTime ModifiedAt { get; set; }

    }
}
