namespace Optima.Fais.Dto
{
    public class RequestBFMaterialCostCenterAdd
    {
        public int[] CostCenterIds { get; set; }
        public int RequestBudgetForecastMaterialId { get; set; }
		public int OrderId { get; set; }
	}
}
