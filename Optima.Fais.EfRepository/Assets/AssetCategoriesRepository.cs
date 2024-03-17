using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;

namespace Optima.Fais.EfRepository
{
    public class AssetCategoriesRepository : Repository<AssetCategory>, IAssetCategoriesRepository
    {
        public AssetCategoriesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }
    }
}
