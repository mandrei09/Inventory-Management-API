using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.LDAP
{
    public class LdapBackgroundService : BackgroundService
    {
        private readonly ILdapSyncService _ldapSyncService = null;

        public LdapBackgroundService(IServiceProvider services,
            ILdapSyncService ldapSyncService)
        {
            Services = services;
            _ldapSyncService = ldapSyncService;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _ldapSyncService.SyncEmployeesAsync();
                }
                catch (Exception ex)
                {
                    //Console.Write("Sync Employee ", ConsoleColor.Red);
                    //Console.Write(ex.Message, ConsoleColor.DarkRed);

                    // for other error types just write the info without the FailedRecipient
                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(ex.StackTrace);
                        errorfile.WriteLine(ex.ToString());

                    };
                }

                await Task.Delay(36000000, stoppingToken);
            }
        }

    }
}
