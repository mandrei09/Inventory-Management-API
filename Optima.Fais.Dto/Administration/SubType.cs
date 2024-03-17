namespace Optima.Fais.Dto
{
    public class SubType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? TypeId { get; set; }
        public CodeNameEntity Type { get; set; }
        public int? MasterTypeId { get; set; }
        public CodeNameEntity MasterType { get; set; }
        public System.DateTime ModifiedAt { get; set; }


    }

    public class SubTypeBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class SubTypeViewResource : SubTypeBase
    {
        public TypeViewResource Type { get; set; }
        public MasterTypeViewResource MasterType { get; set; }
    }
}
