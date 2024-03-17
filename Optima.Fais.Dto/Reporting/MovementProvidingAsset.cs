using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class MovementProvidingAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public int Quantity { get; set; }
        public float Value { get; set; }
        public string AccountingGroup { get; set; }
        public string Location { get; set; }
        public string LocationNameInitial { get; set; }
        public string LocationNameFinal { get; set; }
        public string CostCenterNameInitial { get; set; }
        public string CostCenterNameFinal { get; set; }
        public string Room { get; set; }
        public string RoomCodeInitial { get; set; }
        public string RoomCodeFinal { get; set; }
        public string RoomNameInitial { get; set; }
        public string RoomNameFinal { get; set; }
        public string RegionNameInitial { get; set; }
        public string RegionNameFinal { get; set; }
        public string InternalCodeInitial { get; set; }
        public string InternalCodeFinal { get; set; }
        public string PrefixInitial { get; set; }
        public string PrefixFinal { get; set; }
        public string FirstNameInitial { get; set; }
        public string FirstNameFinal { get; set; }
        public string LastNameInitial { get; set; }
        public string LastNameFinal { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string SerialNumber { get; set; }
        public int TransferPartialNumber { get; set; }
        public int TransferFinalNumber { get; set; }
        public int AssetOpId { get; set; }
    }
}
