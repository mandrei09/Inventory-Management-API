using Optima.Fais.Model;

namespace Optima.Fais.Dto
{
    public class AssetDepMDCapDTOSync
    {
        public int Id { get; set; }

        public CodeNameEntity BudgetManager { get; set; }

        public AccMonth AccMonth { get; set; }

        public string BUKRSH { get; set; }

        public string BLDAT { get; set; }

        public string BUDAT { get; set; }

        public string SGTXT { get; set; }

        public string XBLNR { get; set; }

        public string TCODE { get; set; }



        public string ANLN1 { get; set; }

        public string ANLN2 { get; set; }

        public decimal ANBTR { get; set; }



        public string ANLKL { get; set; }

        public string BUKRST { get; set; }

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

        public bool IsImported { get; set; }
    }
}
