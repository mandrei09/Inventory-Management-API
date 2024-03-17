using System;

namespace Optima.Fais.Dto
{
    public class CountySync
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int? CountryId { get; set; }
        public DateTime ModifiedAt { get; set; }

    }
}
