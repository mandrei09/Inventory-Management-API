namespace Optima.Fais.Dto
{
    public class ConfigValue : ConfigValueBase
    {
        public int Id { get; set; }
		public string RoleId { get; set; }
		public RoleEntity Role { get; set; }
	}
}
