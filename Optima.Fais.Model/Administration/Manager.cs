using System;

namespace Optima.Fais.Model
{
    public partial class Manager : Entity
    {
        public Manager()
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

        public DateTime? NotifyLast { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsEmailSend { get; set; }

		public Guid Guid { get; set; }

		public int AssetCount { get; set; }
	}
}
