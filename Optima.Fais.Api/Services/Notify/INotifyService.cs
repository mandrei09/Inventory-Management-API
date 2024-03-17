using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface INotifyService
    {
        Task NotifyDataCreateAssetAsync(CreateAssetSAPResult asset);
        Task NotifyDataRetireAssetAsync(CreateAssetSAPResult asset);
        Task NotifyDataStornoAssetAsync(CreateAssetSAPResult asset);
        Task NotifyDataStornoAcquisitionAssetAsync(CreateAssetSAPResult asset);
        Task NotifyDataTransferAssetAsync(CreateAssetSAPResult asset);
        Task NotifyDataAssetInvMinusAsync(CreateAssetSAPResult asset);
        Task NotifyDataAssetInvPlusAsync(CreateAssetSAPResult asset);
        Task NotifyDataOrderItemDeleteAsync(AssetResult asset);
        Task NotifyDataEditAssetAsync(UpdateAssetSAPResult asset);
        Task NotifyDataCreateAssetSAPAsync(CreateAssetSAPResult asset);
		Task NotifyDataAcquisitionAssetSAPAsync(CreateAssetSAPResult asset);
		Task NotifyDataWFHValidateAsync(WFHResult asset);
		Task NotifyDataChangeAssetAsync(CreateAssetSAPResult asset);
	}
}
