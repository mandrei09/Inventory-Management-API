using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class EmailUI
    {

        public List<AssetComponent> AssetComponents { get; set; }

        public List<AssetUI> Assets { get; set; }

        public List<AssetComponent> ListComponent()
        {
            return AssetComponents;
        }

        public List<AssetUI> ListAsset()
        {
            return Assets;
        }

    }
}
