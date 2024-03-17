using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Identity
{
    public interface IAppUser
    {
        string UserName { get; }
        string DisplayName { get; }
        string Email { get; }
        string[] Roles { get; }
    }

    public class AppUser : IAppUser
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
