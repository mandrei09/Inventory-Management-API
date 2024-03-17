﻿using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{

    public class AssetStatusDashboard
    {
        public int Id { get; set; }
		public string InvNo { get; set; }
		public string Name { get; set; }
		public string SerialNumber { get; set; }
		public bool isPrinted { get; set; }
		public bool isDuplicate { get; set; }
		public CodeNameEntity DepartmentInitial { get; set; }
		//public CodeNameEntity DepartmentFinal { get; set; }
		//public CodeNameEntity DivisionInitial { get; set; }
		//public CodeNameEntity DivisionFinal { get; set; }
		//public CodeNameEntity CostCenterInitial { get; set; }
		//public CodeNameEntity CostCenterFinal { get; set; }
		public DateTime PurchaseDate { get; set; }
		public DateTime ModifiedAt { get; set; }

	}
}