using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryList
    {
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyUniqueID { get; set; }
        public string CompanyRegistryNumber { get; set; }

        public string Location { get; set; }
        public string EndDate { get; set; }
        public string Administration { get; set; }

        public List<InventoryListAsset> Assets;

        public List<InventoryListAsset> ListInventoryListAssets()
        {
            return Assets;
        }
    }
}
