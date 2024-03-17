namespace Optima.Fais.Dto
{
    public class Country
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string IsDeleted { get; set; }

        public System.DateTime ModifiedAt { get; set; }
    }

    public class CountryViewResource : CodeNameEntity
    {
    }
}
