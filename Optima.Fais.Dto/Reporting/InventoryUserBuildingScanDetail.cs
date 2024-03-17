using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryUserBuildingScanDetail
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public DateTime? SyncDate { get; set; }
        public string SyncHour { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string InventoryName { get; set; }
    }
}
