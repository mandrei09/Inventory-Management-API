using Microsoft.Extensions.Hosting;
using Optima.Fais.Api.Services.SAPAriba;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class ApiSAPAribaService : BackgroundService
	{

        private readonly IContractImportService _contractImportService = null;

        public ApiSAPAribaService(IServiceProvider services,
            IContractImportService contractImportService)
        {
            Services = services;
            _contractImportService = contractImportService;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _contractImportService.ContractImportAsync();
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

                await Task.Delay(36000000, stoppingToken);
            }
        }

        //protected override async Task StopAsync(CancellationToken stoppingToken)
        //{
        //    // Run your graceful clean-up actions
        //}
    }
}
