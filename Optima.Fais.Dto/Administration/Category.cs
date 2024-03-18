using System;

namespace Optima.Fais.Dto
{
    public class Category
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CodeNameEntity CategoryEN { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
