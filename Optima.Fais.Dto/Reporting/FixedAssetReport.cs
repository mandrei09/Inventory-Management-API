using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class FixedAssetReport
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public string InvNo { get; set; }
        public string SourceDocumentType { get; set; }
        public string SourceDocumentDate { get; set; }
        public string SourceDocumentNumber { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public string TechnicalDetails { get; set; }
        public string Accesories { get; set; }
        public string Group { get; set; }
        public string ClassificationCode { get; set; }
        public string CommissioningMonth { get; set; }
        public string CommissioningYear { get; set; }
        public string FullDepreciationMonth { get; set; }
        public string FullDepreciationYear { get; set; }
        public int NormalUsagePeriod { get; set; }
        public float DepreciationRate { get; set; }

        public List<FixedAssetReportAsset> Assets;

        public List<FixedAssetReportAsset> ListFixedAssetReportAssets()
        {
            return Assets;
        }
    }
}
