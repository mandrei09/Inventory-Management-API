using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class RequestBudgetForecastData
    {
        public Request Request { get; set; }
        public IEnumerable<RequestBudgetForecastChildrenBase> Children { get; set; }
    }
}
