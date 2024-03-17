namespace Optima.Fais.Dto
{
    public class OperationUpload
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int? DivisionId { get; set; }
        public int? CostCenterId { get; set; }
        public int? PartnerId { get; set; }
        public int? BudgetManagerId { get; set; }
        public int? ProjectId { get; set; }
        public int? DimensionId { get; set; }
        public int? AssetNatureId { get; set; }
        public int? EmployeeId { get; set; }
        public int? AssetAccStateId { get; set; }
        public int? AssetCategoryId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoomId { get; set; }
        public bool IsDeleted { get; set; }
        public int? EmployeeIdIni { get; set; }
        //public int? OrderId { get; set; }
    }
}
