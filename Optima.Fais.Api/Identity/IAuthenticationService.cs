using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Identity
{
    public interface IAuthenticationService
    {
        //IAppUser Login(string username, string password);
        Task<IAppUser> LoginAsync(string username, string password);
    }
}
