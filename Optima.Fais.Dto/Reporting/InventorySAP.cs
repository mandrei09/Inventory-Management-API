using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventorySAP
    {
        public string InvNo { get; set; }
        public string Description { get; set; }
        public string DescriptionPlus { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterCodeInitial { get; set; }
        public string CostCenterCodeFinal { get; set; }
        public string AdmCenterName { get; set; }
        public string AdmCenterNameInitial { get; set; }
        public string AdmCenterNameFinal { get; set; }
        public string CostCenterName { get; set; }
        public string EmployeeInternalCode { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string SerialNumber { get; set; }
        public string RoomName { get; set; }
        public string InventoryName { get; set; }
        public DateTime InventoryDate { get; set; }

        public List<InventorySAPDetail> SAPDetail;

        public List<InventorySAPDetail> ListInventorySAP()
        {
            return SAPDetail;
        }
    }
}
