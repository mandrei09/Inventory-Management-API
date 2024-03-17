using Microsoft.Extensions.Hosting;
using Optima.Fais.Api.Services.BNR;
using Optima.Fais.Api.Services.SAPAriba;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class BNRRateAPIService : BackgroundService
	{

        private readonly IBNRRatesImportService _bnrRatesImportService = null;

        public BNRRateAPIService(IServiceProvider services,
            IBNRRatesImportService bnrRatesImportService)
        {
            Services = services;
            _bnrRatesImportService = bnrRatesImportService;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _bnrRatesImportService.BNRRatesImportAsync();
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
