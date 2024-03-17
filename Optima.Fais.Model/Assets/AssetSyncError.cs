using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
        public class AssetSyncError :   Entity
        {
            public int AssetId { get; set; }
            public string SyncStatus { get; set; }
            public string SyncCode { get; set; }
            public string SyncDate { get; set; }
            public int Error { get; set; }
            public string ManualSyncErrors { get; set; }
    }
}
