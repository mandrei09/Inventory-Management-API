using System;

namespace Optima.Fais.Model
{
    public partial class Owner : Entity
    {
        public Owner()
        {
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UniqueName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Organization { get; set; }

        public string OrgANId { get; set; }

        public string OrgName { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
