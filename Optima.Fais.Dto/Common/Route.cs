using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    //  public class Route
    //  {
    //      public Route()
    //      {
    //          ListChild = new List<RouteChildren>();
    //      }

    //public int Id { get; set; }
    //public string Name { get; set; }
    //      public string Url { get; set; }
    //      public string Icon { get; set; }
    //      public string RoleId { get; set; }
    //      public List<RouteChildren> ListChild { get; set; }
    //  }

    public class Route : RouteBase
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }
		public bool Divider { get; set; }
        public string Variant { get; set; }
        public bool Active { get; set; }
        public string Class { get; set; }
        public Badge Badge { get; set; }
		public int Position { get; set; }
	}
}
