using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class MultipleAssetUpload
    {

        public string InvNoInitial { get; set; }

        public string InvNoFinal { get; set; }

        public string Name { get; set; }

        public string Producer { get; set; }

        public string Model { get; set; }

        public string Info { get; set; }

        public float Quantity { get; set; }

        public bool AllowLabel { get; set; }

        public int? RoomId { get; set; }

        public int? AdministrationId { get; set; }

        public int? InvStateId { get; set; }

        public int? AssetStateId { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public System.DateTime CreatedAt { get; set; }

        public int? AssetCategoryId { get; set; }

        public int? AssetTypeId { get; set; }

        public int? DepartmentId { get; set; }

        public string SerialNumber { get; set; }

        public decimal ValueInv { get; set; }

        public bool Validated { get; set; }

        public string UserId { get; set; }

    }

}
