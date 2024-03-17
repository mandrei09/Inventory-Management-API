using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class Annulement
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public string DocumentNumber { get; set; }
        public string DocumentDate { get; set;  }

        public List<AnnulementAsset> Assets;

        public List<AnnulementAsset> ListAnnulementAssets()
        {
            return Assets;
        }
    }
}
