using System;

namespace Optima.Fais.Dto
{
    public class AssetComponentOp
    {
        public int Id { get; set; }

        public AssetComponent AssetComponent { get; set; }

        public Document Document { get; set; }

        public CodeNameEntity DocumentType { get; set; }

        public EmployeeResource EmployeeInitial { get; set; }

        public EmployeeResource EmployeeFinal { get; set; }

        public CodeNameEntity InvState { get; set; }

        public DateTime ModifiedAt { get; set; }
      
    }
}
