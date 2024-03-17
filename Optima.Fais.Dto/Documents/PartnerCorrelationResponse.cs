using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class PartnerCorrelationResponse
	{
		public string Cod { get; set; }
		public string Message { get; set; }
		public List<Found> Found { get; set; }
	}
}


public class Found
{
	public string Cui { get; set; }
	public string Data { get; set; }
	public string Denumire { get; set; }
	public string Adresa { get; set; }
	public string NrRegCom { get; set; }
	public string Telefon { get; set; }
	public string Fax { get; set; }
	public string CodPostal { get; set; }
	public string Act { get; set; }
	public string Stare_inregistrare { get; set; }
	public bool ScpTVA { get; set; }
	public string Data_inceput_ScpTVA { get; set; }
	public string Data_sfarsit_ScpTVA { get; set; }
	public string Data_anul_imp_ScpTVA { get; set; }
	public string Mesaj_ScpTVA { get; set; }
	public string DataInceputTvaInc { get; set; }
	public string DataSfarsitTvaInc { get; set; }
	public string DataActualizareTvaInc { get; set; }
	public string DataPublicareTvaInc { get; set; }
	public string TipActTvaInc { get; set; }
	public bool StatusTvaIncasare { get; set; }
	public string DataInactivare { get; set; }
	public string DataReactivare { get; set; }
	public string DataPublicare { get; set; }
	public string DataRadiere { get; set; }
	public bool StatusInactivi { get; set; }
	public string DataInceputSplitTVA { get; set; }
	public string DataAnulareSplitTVA { get; set; }
	public bool StatusSplitTVA { get; set; }
	public string Iban { get; set; }
	public bool StatusRO_e_Factura { get; set; }
}
