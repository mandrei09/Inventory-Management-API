namespace Optima.Fais.Dto
{
    public class Location
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? RegionId { get; set; }
        public int? CostCenterId { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public CodeNameEntity Region { get; set; }
        public System.DateTime ModifiedAt { get; set; }
        public int? AdmCenterId { get; set; }
        public CodeNameEntity AdmCenter { get; set; }
        public int? CityId { get; set; }
        public CodeNameEntity City { get; set; }
        public CodeNameEntity County { get; set; }
        public int? LocationTypeId { get; set; }
        public CodeNameEntity LocationType { get; set; }
        public int? EmployeeId { get; set; }
        public EmployeeResource Employee { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Member1 { get; set; }
        public string Member2 { get; set; }
        public string Member3 { get; set; }

    }

    public class LocationBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class LocationViewResource : LocationBase
    {
        public AdmCenterViewResource AdmCenter { get; set; }
        public RegionViewResource Region { get; set; }
    }
}
