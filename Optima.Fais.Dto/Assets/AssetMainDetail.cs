using System;

namespace Optima.Fais.Dto
{
    public class AssetMainDetail
    {
        public int Id { get; set; }
        public string InvNo { get; set; }
        public string Name { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? DocumentId { get; set; }
        public string DocNo1 { get; set; }
        public string DocNo2 { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentType { get; set; }

        public int? PartnerId { get; set; }
        public string Partner { get; set; }

        public int? AssetTypeId { get; set; }
        public string AssetType { get; set; }

        public int? AssetStateId { get; set; }
        public string AssetState { get; set; }

        public int? CostCenterId { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public int? AssetCategoryId { get; set; }
        public string AssetCategory { get; set; }

        public int? DepartmentId { get; set; }
        public string Department { get; set; }

        public int? EmployeeId { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public int? RoomId { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }

        public string SerialNumber { get; set; }

        public bool? Validated { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool? Custody { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}

//using System;

//namespace Optima.Fais.Dto
//{
//    public class AssetMainDetail
//    {
//        public int Id { get; set; }
//        public string InvNo { get; set; }
//        public string Name { get; set; }

//        public DateTime? PurchaseDate { get; set; }

//        public int? DocumentId { get; set; }
//        public string DocNo1 { get; set; }
//        public string DocNo2 { get; set; }
//        public DateTime? DocumentDate { get; set; }

//        public CodeNameEntity Partner { get; set; }
//        public CodeNameEntity DocumentType { get; set; }
//        public CodeNameEntity AssetType { get; set; }
//        public CodeNameEntity AssetState { get; set; }
//        public CodeNameEntity AssetCategory { get; set; }

//        public CodeNameEntity AdmCenter { get; set; }
//        public CodeNameEntity CostCenter { get; set; }
//        public CodeNameEntity Region { get; set; }
//        public CodeNameEntity Location { get; set; }
//        public CodeNameEntity Room { get; set; }
//        public CodeNameEntity Department { get; set; }
//        public EmployeeResource Employee { get; set; }

//        public string SerialNumber { get; set; }

//        public bool? Validated { get; set; }

//        public DateTime CreatedAt { get; set; }
//        public bool? Custody { get; set; }
//        public DateTime? ModifiedAt { get; set; }
//    }
//}
