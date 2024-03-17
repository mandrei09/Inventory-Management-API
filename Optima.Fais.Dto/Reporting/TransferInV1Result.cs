using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class TransferInV1Result
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

        public string ReportType { get; set; }

        public string Provider { get; set; }

        public DateTime EndDate { get; set; }
        public string InventoryName { get; set; }
        public DateTime InventoryEndDate { get; set; }

        public List<TransferInV1Detail> Details;

        public List<TransferInV1Detail> ListTransferInV1Detail()
        {
            return Details;
        }
    }
}
