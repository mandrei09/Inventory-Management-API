﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Common
{
    public class RequestBudgetForecastMaterialUpdate
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal ValueRon { get; set; }
    }
}