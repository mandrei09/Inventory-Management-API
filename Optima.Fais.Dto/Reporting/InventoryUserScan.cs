using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryUserScan
    {
        public string InvNo { get; set; }
        public string Description { get; set; }
        public string AdmCenterCode { get; set; }
        public string AdmCenterName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string EmployeeInternalCode { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFullName { get; set; }
        public DateTime? SyncDate { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string InventoryName { get; set; }

        public List<InventoryUserScanDetail> UserScanDetail;

        public List<InventoryUserScanDetail> ListInventoryUserScan()
        {
            return UserScanDetail;
        }
    }
}
