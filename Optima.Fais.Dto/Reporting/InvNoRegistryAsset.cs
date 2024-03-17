using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class InvNoRegistryAsset
    {
        public string InvNo { get; set; }
        public string Name { get; set; }
        public string AssetCategory { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Observations { get; set; }
    }
}
