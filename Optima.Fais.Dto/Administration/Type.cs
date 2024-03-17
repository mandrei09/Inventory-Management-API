namespace Optima.Fais.Dto
{
    public class Type
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? MasterTypeId { get; set; }
        public CodeNameEntity MasterType { get; set; }
        public System.DateTime ModifiedAt { get; set; }


    }

    public class TypeBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class TypeViewResource : TypeBase
    {
        public MasterTypeViewResource MasterType { get; set; }
    }
}
