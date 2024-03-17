using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class ResponsableTotal
	{
		[Key]
		public int Id { get; set; }
		public string UserName { get; set; }
        public int Items { get; set; }
        public DateTime ScanDate { get; set; }
        //public string Name { get; set; }
        //public string Room { get; set; }
        //public string Administration { get; set; }
        //public string Division { get; set; }
        //public string Department { get; set; }

        //public int Initial { get; set; }
        //public decimal InitialCurrentAPC { get; set; }
        //public decimal InitialCurrBkValue { get; set; }
        //public decimal InitialAccumulDep { get; set; }

        //public int Scanned { get; set; }
        //public decimal ScannedCurrentAPC { get; set; }
        //public decimal ScannedCurrBkValue { get; set; }
        //public decimal ScannedAccumulDep { get; set; }


        //public int NotScanned { get; set; }
        //public decimal NotScannedCurrentAPC { get; set; }
        //public decimal NotScannedCurrBkValue { get; set; }
        //public decimal NotScannedAccumulDep { get; set; }

  //      public int Temp { get; set; }

		//public decimal Procentage { get; set; }
		//public DateTime? LastScan { get; set; }
	}
}