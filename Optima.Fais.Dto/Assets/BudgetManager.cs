namespace Optima.Fais.Dto
{
    public class BudgetManager
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? UomId { get; set; }
        public CodeNameEntity Uom { get; set; }
        public System.DateTime ModifiedAt { get; set; }


    }

    public class BudgetManagerBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class BudgetManagerViewResource : BudgetManagerBase
    {
        public UomViewResource Uom { get; set; }
    }
}
