using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryListApaNova
    {
        public string LocationNameInitial { get; set; }
        public string LocationNameFinal { get; set; }
        public string AssetCategory { get; set; }
        public string InvNo { get; set; }
        public string InvNoParent { get; set; }
        public string Description { get; set; }
        public float Qinitial { get; set; }
        public float QFinal { get; set; }
        public string Um { get; set; }
        public string CostCenterNameInitial { get; set; }
        public string CostCenterNameFinal { get; set; }
        public string AssetStateInitial { get; set; }
        public string AssetStateFinal { get; set; }
        public string UserEmployeeFullNameInitial { get; set; }
        public string UserEmployeeInternalCodeInitial { get; set; }
        public string UserEmployeeFullNameFinal { get; set; }
        public string UserEmployeeInternalCodeFinal { get; set; }
        public string StreetCodeInitial { get; set; }
        public string StreetNameInitial { get; set; }
        public string StreetCodeFinal { get; set; }
        public string StreetNameFinal { get; set; }
        public string RoomCodeInitial { get; set; }
        public string RoomNameInitial { get; set; }
        public string RoomCodeFinal { get; set; }
        public string RoomNameFinal { get; set; }
        public string SerialNumber { get; set; }
        public string Info { get; set; }
        public string GpsCoordinates { get; set; }

        public DateTime? InventoryDate { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public List<InventoryListDetailApaNova> InventoryListDetailInternMod;

        public List<InventoryListDetailApaNova> ListInventoryMiInternMod()
        {
            return InventoryListDetailInternMod;
        }
    }
}
