using System;

namespace Optima.Fais.Dto
{
    public class AssetInvDetail //: AssetMainDetail
    {
        public string Producer { get; set; }

        public string Model { get; set; }


        public string SerialNumber { get; set; }
      
        public string Info { get; set; }
        public string Info2019 { get; set; }
        public bool AllowLabel { get; set; }

        public float Quantity { get; set; }
        public virtual CodeNameEntity State { get; set; }


        //public int? InvStateId { get; set; }

        // public CodeNameEntity InvState { get; set; }

    }
}
