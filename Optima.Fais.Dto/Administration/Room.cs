namespace Optima.Fais.Dto
{
    public class Room
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public CodeNameEntity Location { get; set; }

        public CodeNameEntity Region { get; set; }

        public CodeNameEntity AdmCenter { get; set; }

        public EmployeeResource Employee { get; set; }

        public CodeNameEntity City { get; set; }
        public CodeNameEntity County { get; set; }

        public System.DateTime ModifiedAt { get; set; }

        public bool IsFinished { get; set; }
    }

    public class RoomViewResource : CodeNameEntity
    {
        public LocationViewResource Location { get; set; }

        public bool IsDeleted { get; set; }
    }
}
