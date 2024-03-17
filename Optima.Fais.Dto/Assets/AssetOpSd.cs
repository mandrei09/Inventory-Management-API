using System;

namespace Optima.Fais.Dto
{
    public class AssetOpSd
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentDetails { get; set; }

        public int? RoomId { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }

        public int? LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public int? EmployeeId { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string StateCode { get; set; }
        public string State { get; set; }
        public DateTime? ValidationDate { get; set; }
        public string InvNo { get; set; }
      
        public DateTime? ReleaseConfAt { get; set; }
        public string ReleaseConfBy { get; set; }
        public DateTime? SrcConfAt { get; set; }
        public string SrcConfBy { get; set; }
        public DateTime? DstConfAt { get; set; }
        public string DstConfBy { get; set; }
        public DateTime? RegisterConfAt { get; set; }
        public string RegisterConfBy { get; set; }
        public int? AssetOpStateId { get; set; }
        public string AssetOpStateCode { get; set; }
        public string AssetOpStateName { get; set; }

    }
}
