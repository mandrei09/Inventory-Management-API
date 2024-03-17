namespace Optima.Fais.Dto
{
	public class MasterType
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }

		public System.DateTime ModifiedAt { get; set; }
	}

    public class MasterTypeViewResource : CodeNameEntity
    {
    }
}
