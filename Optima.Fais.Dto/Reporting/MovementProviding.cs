using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class MovementProviding
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public string ProviderAdministration { get; set; }
        public string ReceiverAdministration { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentDate { get; set; }

        public string SourceDocumentNumber { get; set; }
        public string SourceDocumentDate { get; set; }
        public int?   assetOpId { get; set; }

        public List<MovementProvidingAsset> Assets;
        public List<ReportAsset> ReportAssets;
    }
}
