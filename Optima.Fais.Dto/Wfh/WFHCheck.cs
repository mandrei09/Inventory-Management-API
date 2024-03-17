using Optima.Fais.Model;

namespace Optima.Fais.Dto
{
    public class WFHCheck
	{
        public int Id { get; set; }

		public DictionaryItem DictionaryItem { get; set; }

		public Brand Brand { get; set; }

		public Model Model { get; set; }

		public string Imei { get; set; }

		public string SerialNumber { get; set; }

		public string InventoryNumber { get; set; }

		public BudgetManager BudgetManager { get; set; }

		public Employee Employee { get; set; }
	}
}
