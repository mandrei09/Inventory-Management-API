namespace Optima.Fais.Dto
{
    public class RequestBudgetForecastMaterialAdd
    {
        public int[] MaterialIds { get; set; }
        public int RequestBudgetForecastId { get; set; }
		public int OfferId { get; set; }
    }
}
