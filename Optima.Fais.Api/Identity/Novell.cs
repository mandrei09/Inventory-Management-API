using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Identity
{
    public class LdapConfig
    {
        public string Url { get; set; }
        public string Port { get; set; }
        public string BindDn { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }

        public string SearchAllFilter { get; set; }

        public string FirstNameAttribute { get; set; }
        public string LastNameAttribute { get; set; }
        public string FullNameAttribute { get; set; }
        public string EmailAttribute { get; set; }
        public string DepartmentAttribute { get; set; }
        public string IsActiveAttribute { get; set; }
        public string InternalCodeAttribute { get; set; }
        public string CostCenterAttribute { get; set; }
        public string ManagerAttribute { get; set; }
        public string CompanyAttribute { get; set; }

        public string MemberOfAttribute { get; set; }
        public string DisplayNameAttribute { get; set; }
        public string SAMAccountNameAttribute { get; set; }
    }
}
