using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class TransferOutV1Result
    {
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

        public DateTime EndDate { get; set; }

        public string InventoryName { get; set; }
        public DateTime InventoryEndDate { get; set; }

        public List<TransferOutV1Detail> Details;

        public List<TransferOutV1Detail> ListTransferOutV1Detail()
        {
            return Details;
        }
    }
}
