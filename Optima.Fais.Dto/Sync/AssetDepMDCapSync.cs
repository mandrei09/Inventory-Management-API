using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetDepMDCapSync
	{
        public Header Header { get; set; }

        public IList<Tt_Item> Tt_Item { get; set; }
    }


    public class Header
    {
        public string BUKRS { get; set; }

        public string BLDAT { get; set; }

        public string BUDAT { get; set; }

        public string SGTXT { get; set; }

        public string XBLNR { get; set; }

        public string TCODE { get; set; }
    }

    public class Tt_Item
    {
        public string ANLN1 { get; set; }

        public string ANLN2 { get; set; }

        public decimal ANBTR { get; set; } = 0;

        public FA_MD FA_MD { get; set; }
    }

    public class FA_MD
    {
        public string ANLKL { get; set; }

        public string BUKRS { get; set; }

        public string TXT50 { get; set; }

        public string TXA50 { get; set; }

        public string SERNR { get; set; }

        public string AKTIV { get; set; }

        public string KOSTL { get; set; }

        public string CAUFN { get; set; }

        public string KOSTLV { get; set; }

        public string WERKS { get; set; }

        public string STORT { get; set; }

        public string KFZKZ { get; set; }

        public string ZZCLAS { get; set; }

        public string XSTIL { get; set; }
    }
}
