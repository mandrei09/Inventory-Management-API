namespace Optima.Fais.Model
{
    public class DocumentType : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string ParentCode { get; set; }

        public string Mask { get; set; }

        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }
    }
}
