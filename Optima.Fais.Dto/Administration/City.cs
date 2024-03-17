namespace Optima.Fais.Dto
{
    public class City
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CountyId { get; set; }
        public CodeNameEntity County { get; set; }
        public CodeNameEntity Country { get; set; }
        public System.DateTime ModifiedAt { get; set; }

    }

    public class CityBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CityViewResource : CityBase
    {
        public CityViewResource City { get; set; }
    }
}
