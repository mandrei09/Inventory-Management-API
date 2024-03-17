using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class MovementReport
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public List<MovementReportAsset> Assets;

        public List<MovementReportAsset> ListMovementReportAssets()
        {
            return Assets;
        }
    }
}
