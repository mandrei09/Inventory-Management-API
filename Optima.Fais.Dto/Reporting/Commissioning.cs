using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class Commissioning
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public string DocumentNumber { get; set; }
        public string DocumentDate { get; set;  }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public List<CommissioningAsset> Assets;

        public List<CommissioningAsset> ListCommissioningAssets()
        {
            return Assets;
        }

    }
}
