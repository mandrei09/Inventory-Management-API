using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Employee : Entity
    {
        public Employee()
        {
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string InternalCode { get; set; }

        public string Email { get; set; }

        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string ERPCode { get; set; }

        public int? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        //public int? LocationId { get; set; }

        //public virtual Location Location { get; set; }

        public int? DivisionId { get; set; }

        public Division Division { get; set; }

        public DateTime? NotifyLast { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsEmailSend { get; set; }

		public int? ERPId { get; set; }

		public Guid Guid { get; set; }

		public int AssetCount { get; set; }

        public int? ManagerId { get; set; }

        public Manager Manager { get; set; }

        public bool IsBudgetOwner { get; set; }

        ////sync
        //public int ClientId { get; set; }

        //public Guid CreatedBy { get; set; }

        //public DateTime DateCreated { get; set; }

        //public Guid LastModifiedBy { get; set; }

        //public DateTime LastModified { get; set; }
        ////end sync
    }
}
