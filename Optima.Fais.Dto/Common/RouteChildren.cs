using Optima.Fais.Model;

namespace Optima.Fais.Dto
{
	//public class RouteChildren
	//{
	//       public string Name { get; set; }
	//       public string Url { get; set; }
	//       public string Icon { get; set; }
	//       public int? RouteId { get; set; }
	//       public Route Route { get; set; }
	//   }

	public class RouteChildren : RouteBase
	{
		public int Id { get; set; }
		public int RouteId { get; set; }
		public CodeNameEntity Route { get; set; }
		public string RoleId { get; set; }
		public RoleEntity Role { get; set; }
		public int Position { get; set; }
		//public string HeaderCode { get; set; }
		//public string Property { get; set; }
		//public string Include { get; set; }
		//public string SortBy { get; set; }
		//public string Pipe { get; set; }
		//public string Format { get; set; }
		//public string TextAlign { get; set; }
		//public bool Active { get; set; }
		//public int Position { get; set; }
	}
}
