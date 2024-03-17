using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
   public class ApplicationUser
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Role { get; set; }

        //public CodeNameEntity AdmCenter { get; set; }

        public EmployeeResource Employee { get; set; }

        //public string AdmCenters { get; set; }

        //public string CompanyTypes { get; set; }

        //public string Locations { get; set; }

        public Device Device { get; set; }

        public string Mac { get; set; }

        public EmployeeResource Substitute { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

		public DateTime? LastLogin { get; set; }

		public MobilePhone MobilePhone { get; set; }
	}
}
