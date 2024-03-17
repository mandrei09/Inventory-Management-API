using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class EmailRequestStatus
    {
        public int Id { get; set; }
		public int DocumentNumber { get; set; }
        public int? RequestBudgetForecastId { get; set; }
        public int RequestId { get; set; }
    }
}
