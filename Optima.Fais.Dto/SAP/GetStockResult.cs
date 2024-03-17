using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class GetStockResult
	{
		public Meta Meta { get; set; }
		public DataOutPut Data { get; set; }
	}
}

public class Meta
{
	public int Code { get; set; }
}

public class DataOutPut
{
	public List<OutPut> E_OutPut { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}

public class OutPut
{
	public string Material { get; set; }
	public string Short_Descr { get; set; }
	public string Batch { get; set; }
	public string CompanyCode { get; set; }
	public string Plant { get; set; }
	public string Storage_Location { get; set; }
	public decimal Quantity { get; set; }
	public string Uom { get; set; }
	public decimal UnitCost { get; set; }
	public string Currency { get; set; }
	public string Last_Incoming_Date { get; set; }
	public string SupplierId { get; set; }
	public string SupplierName { get; set; }
	public string Producer { get; set; }
	public string Long_Descr { get; set; }
	public string Category { get; set; }
	public string Category_Descr { get; set; }
	public string EAN { get; set; }
	public string Invoice{ get; set; }

	public string Plant_Initial { get; set; }
	public string Storage_Location_Initial { get; set; }














}



//{
//    "meta": {
//        "code": 200
//    },
//    "data": {
//        "E_OUTPUT": [
//            {
//            "MATERIAL": "P810871           ",
//                "SHORT_DESCR": "Laptop K9K67EA                          ",
//                "BATCH": "0000502863",
//                "COMPANYCODE": "RO10",
//                "PLANT": "RO02",
//                "STORAGE_LOCATION": "MFX ",
//                "QUANTITY": 10,
//                "UOM": "ST ",
//                "UNITCOST": 1992.71,
//                "CURRENCY": "RON  ",
//                "LAST_INCOMING_DATE": "20150602",
//                "SUPPLIERID": "0000000004",
//                "SUPPLIERNAME": "Network One Distribution SRL.                                                                                                                         ",
//                "PRODUCER": "HP                                                                                                                                                                                                                                                             ",
//                "LONG_DESCR": "Laptop HP ProBook 450 G2 cu procesor Intel® Core™ i5-5200U 2.20GHz, Broadwell™, 15.6\", 4GB, 1TB, DVD-RW, AMD Radeon™ R5 M255 2GB, Fr                                                                                                                                                                        ",
//                "CATEGORY": "91       ",
//                "CATEGORY_DESCR": "LAPTOP / NOTEBOOK   "
//            }
//        ],
//        "RETURN_CODE": "1",
//        "RETURN_MESSAGE": "Succes"
//    }
//}
