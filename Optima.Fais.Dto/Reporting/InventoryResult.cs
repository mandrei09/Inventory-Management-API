using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryResult
    {
        public string InventoryListType { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public string AdmCenterName { get; set; }
        public string RegionName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AdministrationCode { get; set; }
        public string AdministrationName { get; set; }

        public string Provider { get; set; }

        public string EmployeeInternalCode { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

        public string InventoryName { get; set; }
        public string Committee1 { get; set; }
        public string Committee2 { get; set; }
        public string Committee3 { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime InventoryEndDate { get; set; }

        public List<InventoryResultDetail> Details;

        public List<AssetAdmDetail> AdmDetails;

        public List<InventoryResultDetail> ListInventoryResultDetail()
        {
            return Details;
        }
        public List<AssetAdmDetail> ListAdmResultDetail()
        {
            return AdmDetails;
        }
    }
}
