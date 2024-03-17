using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRoomsRepository : IRepository<Room>
    {
        int GetCountByFilters(string filter, List<int> locationIds, List<int> regionIds, List<int> admCenterIds);
        IEnumerable<Room> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> locationIds, List<int> regionIds, List<int> admCenterIds);
        //IEnumerable<Dto.RoomDetail> GetDetailsByFilters(int? locationId, string filter, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
        IEnumerable<Model.Room> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
