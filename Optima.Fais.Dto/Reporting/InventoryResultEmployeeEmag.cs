﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryResultEmployeeEmag
    {
        public string Company { get; set; }
        public string Address { get; set; }
        public string Administration { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string InventoryName { get; set; }
        public string Member1 { get; set; }
        public string Member2 { get; set; }
        public string Member3 { get; set; }
        public string Member4 { get; set; }
        public string Member5 { get; set; }
        public string Document { get; set; }

        public string Employee { get; set; }

        public List<InventoryResultDetailEmployeeEmag> Details;

        public List<InventoryResultDetailEmployeeEmag> ListInventoryResultDetail()
        {
            return Details;
        }
    }
}