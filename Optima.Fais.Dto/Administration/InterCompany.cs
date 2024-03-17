namespace Optima.Fais.Dto
{
    public class InterCompany
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CodeNameEntity InterCompanyEN { get; set; }
        public System.DateTime ModifiedAt { get; set; }


    }

    public class InterCompanyBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class InterCompanyViewResource : InterCompanyBase
    {
        public PartnerViewResource Uom { get; set; }
    }
}
