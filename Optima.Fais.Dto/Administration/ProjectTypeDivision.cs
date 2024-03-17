using System;

namespace Optima.Fais.Dto
{
    public class ProjectTypeDivision
    {
        public int Id { get; set; }
        public CodeNameEntity ProjectType { get; set; }
        public CodeNameEntity Division { get; set; }
        public CodeNameEntity Department { get; set; }
    }
}
