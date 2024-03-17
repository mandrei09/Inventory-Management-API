using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;

namespace Optima.Fais.EfRepository
{
    public class PartnerLocationsRepository : Repository<PartnerLocation>, IPartnerLocationsRepository
    {
        public PartnerLocationsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (p) => (p.Denumire.Contains(filter) || p.Cui.Contains(filter) || p.CodPostal.Contains(filter)); })
        { }
    }
}
