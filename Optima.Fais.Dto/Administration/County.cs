namespace Optima.Fais.Dto
{
    public class County
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CountryId { get; set; }
        public CodeNameEntity Country { get; set; }
        public System.DateTime ModifiedAt { get; set; }

    }

    public class CountyBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CountyViewResource : CountyBase
    {
        public CountryViewResource Country { get; set; }
    }
}
