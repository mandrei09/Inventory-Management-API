using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class EmployeeNotValidatedService : IEmployeeNotValidatedService
    {

        public EmployeeNotValidatedService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        public async Task<List<Model.EmployeeNotValidatedEmailResult>> GetEmployeesNotValidatedAsync()
        {

            using (var scope = Services.CreateScope())
            {
                var dbContext =
                   scope.ServiceProvider
                       .GetRequiredService<ApplicationDbContext>();



                List<Model.EmployeeNotValidatedEmailResult> employees = await dbContext.Set<Model.EmployeeNotValidatedEmailResult>().FromSql("GetEmployeeListEmailNotValidated").ToListAsync();

                return employees;
            }

        }

    }
}

