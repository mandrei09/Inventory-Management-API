using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Model
{
	public class ActivityTotal
	{
        public ActivityTotal()
        {
			Freeze = new List<ActivityTotal>();

		}

        [Key]
		public int Id { get; set; }
		public string ActivityCode { get; set; }
        public string AdmCenterCode { get; set; }
		public string AppStateCode { get; set; }
		public decimal Total { get; set; }
		public decimal TotalRem { get; set; }
		List<ActivityTotal> Freeze { get; set; }
		//public string Administration { get; set; }
		//public string Division { get; set; }
		//public string Department { get; set; }

		//public int Initial { get; set; }
		//public int InitialIT { get; set; }
		//public int InitialFacility { get; set; }

		//public decimal InitialCurrBkValue { get; set; }
		//public decimal InitialAccumulDep { get; set; }

		//public decimal InitialITCurrentAPC { get; set; }
		//public decimal InitialITCurrBkValue { get; set; }
		//public decimal InitialITAccumulDep { get; set; }
		//public decimal InitialFacilityCurrentAPC { get; set; }
		//public decimal InitialFacilityCurrBkValue { get; set; }
		//public decimal InitialFacilityAccumulDep { get; set; }

		//public int Scanned { get; set; }
		//public int ScannedIT { get; set; }
		//public int ScannedFacility { get; set; }
		//public decimal ScannedCurrentAPC { get; set; }
		//public decimal ScannedCurrBkValue { get; set; }
		//public decimal ScannedAccumulDep { get; set; }

		//public decimal ScannedITCurrentAPC { get; set; }
		//public decimal ScannedITCurrBkValue { get; set; }
		//public decimal ScannedITAccumulDep { get; set; }
		//public decimal ScannedFacilityCurrentAPC { get; set; }
		//public decimal ScannedFacilityCurrBkValue { get; set; }
		//public decimal ScannedFacilityAccumulDep { get; set; }


		//public int NotScanned { get; set; }
		//public int NotScannedIT { get; set; }
		//public int NotScannedFacility { get; set; }
		//public decimal NotScannedCurrentAPC { get; set; }
		//public decimal NotScannedCurrBkValue { get; set; }
		//public decimal NotScannedAccumulDep { get; set; }

		//public decimal NotScannedITCurrentAPC { get; set; }
		//public decimal NotScannedITCurrBkValue { get; set; }
		//public decimal NotScannedITAccumulDep { get; set; }
		//public decimal NotScannedFacilityCurrentAPC { get; set; }
		//public decimal NotScannedFacilityCurrBkValue { get; set; }
		//public decimal NotScannedFacilityAccumulDep { get; set; }

		//public int Temp { get; set; }

		//public decimal Procentage { get; set; }
		//public DateTime? LastScan { get; set; }

		//[NotMapped]
		//      public string Responsable { get; set; }

	}
}