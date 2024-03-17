using Microsoft.AspNetCore.SignalR;
using Optima.Fais.Api.HubConfig;
using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class NotifyService : INotifyService
    {
        private IHubContext<NotifyHub> _hubContext;

        public NotifyService(IHubContext<NotifyHub> hubContext)
        {
            this._hubContext = hubContext;
        }

		public Task NotifyDataCreateAssetAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetCreateListener", asset);
		}

		public Task NotifyDataRetireAssetAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetRetireListener", asset);
		}

		public Task NotifyDataStornoAssetAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetStornoListener", asset);
		}

		public Task NotifyDataStornoAcquisitionAssetAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetStornoAcquisitionListener", asset);
		}

		public Task NotifyDataTransferAssetAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetTransferListener", asset);
		}

		public Task NotifyDataAssetInvMinusAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetInvMinusListener", asset);
		}

		public Task NotifyDataAssetInvPlusAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("newAssetInvPlusListener", asset);
		}

		public Task NotifyDataOrderItemDeleteAsync(AssetResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("orderItemDeleteListener", asset);
		}

		public Task NotifyDataEditAssetAsync(UpdateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("editAssetListener", asset);
		}

		public Task NotifyDataCreateAssetSAPAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("createAssetSAPListener", asset);
		}

		public Task NotifyDataAcquisitionAssetSAPAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("acquisitionAssetSAPListener", asset);
		}

		public Task NotifyDataWFHValidateAsync(WFHResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("wfhValidateListener", asset);
		}

		public Task NotifyDataChangeAssetAsync(CreateAssetSAPResult asset)
		{
			return this._hubContext.Clients.All.SendAsync("changeAssetSAPListener", asset);
		}
	}
}
