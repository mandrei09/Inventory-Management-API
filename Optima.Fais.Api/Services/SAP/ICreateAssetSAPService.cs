using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface ICreateAssetSAPService
	{
		Task<Model.CreateAssetSAPResult> SearchNewAssetSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewTransferAssetSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewRetireAssetSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewAcquisitionAssetSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewAssetInvPlusSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewAssetInvMinusSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewTransferInStockSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewStornoTransferInStockSAPAsync();
		Task<Model.CreateAssetSAPResult> SearchNewAssetChangeSAPAsync();
	}
}
