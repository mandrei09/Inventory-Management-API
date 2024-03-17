using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetUI : AssetBase
    {

        public CodeNameEntity Room { get; set; }
        public CodeNameEntity Location { get; set; }
        public CodeNameEntity Region { get; set; }
        public CodeNameEntity Type { get; set; }
        public CodeNameEntity Model { get; set; }
        public bool IsAccepted { get; set; }
        public string ImageName { get; set; }

    }
}
