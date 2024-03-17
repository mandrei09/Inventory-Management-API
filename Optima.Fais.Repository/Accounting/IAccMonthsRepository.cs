using Optima.Fais.Model;

namespace Optima.Fais.Repository
{
    public interface IAccMonthsRepository : IRepository<AccMonth>
    {
        AccMonth GetAccMonth(int month, int year);
        int CreateAccMonth(Dto.AccMonth accMonth);
    }
}
