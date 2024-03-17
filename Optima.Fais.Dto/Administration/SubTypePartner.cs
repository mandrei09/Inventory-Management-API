namespace Optima.Fais.Dto
{
    public class SubTypePartner
    {
        public int Id { get; set; }
        public int? SubTypeId { get; set; }
        public CodeNameEntity SubType { get; set; }
        public int? PartnerId { get; set; }
        public CodeNameEntity Partner { get; set; }
        public System.DateTime ModifiedAt { get; set; }


    }

    public class SubTypePartnerBase
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class SubTypePartnerViewResource : SubTypePartnerBase
    {
        //public TypeViewResource Type { get; set; }
        //public MasterTypeViewResource MasterType { get; set; }
    }
}
