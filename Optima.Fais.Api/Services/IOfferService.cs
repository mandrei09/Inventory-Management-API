using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface IOfferService
    {
		Task<Model.OfferResult> SearchNewEmailOfferAsync();
	}
}
