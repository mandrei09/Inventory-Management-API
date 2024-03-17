using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.LDAP
{
    public class LdapEmployee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string InternalCode { get; set; }
        public string Department { get; set; }
        public string CostCenter { get; set; }
        public string Manager { get; set; }
        public string Company { get; set; }
        public bool IsActive { get; set; }
    }
}
