namespace Optima.Fais.Dto
{
	public class Model
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public CodeNameEntity Brand { get; set; }

		public System.DateTime ModifiedAt { get; set; }

        public int SNLength { get; set; }

        public int IMEILength { get; set; }
    }
}
