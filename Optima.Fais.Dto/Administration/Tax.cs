namespace Optima.Fais.Dto
{
	public class Tax
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public decimal Value { get; set; }

		public System.DateTime ModifiedAt { get; set; }
	}
}
