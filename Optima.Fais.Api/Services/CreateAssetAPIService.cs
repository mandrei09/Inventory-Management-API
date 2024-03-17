using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class CreateAssetAPIService : BackgroundService
	{

        private readonly ICreateAssetSAPService _createAssetSAPService = null;

        public CreateAssetAPIService(IServiceProvider services,
            ICreateAssetSAPService createAssetSAPService)
        {
            Services = services;
            _createAssetSAPService = createAssetSAPService;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _createAssetSAPService.SearchNewRetireAssetSAPAsync();
                    await _createAssetSAPService.SearchNewAssetSAPAsync();
                    await _createAssetSAPService.SearchNewAcquisitionAssetSAPAsync();
                    await _createAssetSAPService.SearchNewAssetChangeSAPAsync();

                    await _createAssetSAPService.SearchNewTransferInStockSAPAsync();
                    await _createAssetSAPService.SearchNewStornoTransferInStockSAPAsync();
                    await _createAssetSAPService.SearchNewAssetInvPlusSAPAsync();
                    await _createAssetSAPService.SearchNewAssetInvMinusSAPAsync();
                    await _createAssetSAPService.SearchNewTransferAssetSAPAsync();
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

                await Task.Delay(150000, stoppingToken);
            }
        }

        //protected override async Task StopAsync(CancellationToken stoppingToken)
        //{
        //    // Run your graceful clean-up actions
        //}
    }
}
