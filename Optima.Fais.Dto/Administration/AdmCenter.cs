namespace Optima.Fais.Dto
{
    public class AdmCenter
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int? EmployeeId { get; set; }
        public EmployeeResource Employee { get; set; }
    }

    public class AdmCenterViewResource : CodeNameEntity
    {
    }
}
