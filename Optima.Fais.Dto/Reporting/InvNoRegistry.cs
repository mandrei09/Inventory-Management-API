using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class InvNoRegistry
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public List<InvNoRegistryAsset> Assets;

        public List<InvNoRegistryAsset> ListInvNoRegistryAsset()
        {
            return Assets;
        }
    }
}
