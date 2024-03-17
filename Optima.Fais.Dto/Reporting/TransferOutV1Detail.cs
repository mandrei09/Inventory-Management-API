using System;

namespace Optima.Fais.Dto.Reporting
{
    public class TransferOutV1Detail
    {
        public string Description { get; set; }
        public string InvNo { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal Value { get; set; }
        public decimal ValueDep { get; set; }
        public string RegionName { get; set; }
        public string LocationName { get; set; }
        public string CostCenter { get; set; }
        public string Room { get; set; }
        public string EmployeeInternalCode { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string Info { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string SerialNumber { get; set; }
    }
}