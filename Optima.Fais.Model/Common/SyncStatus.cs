using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class SyncStatus : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool SyncEnabled { get; set; }

        public DateTime? SyncStart { get; set; }

        public DateTime? SyncEnd { get; set; }

        public int SyncInterval { get; set; }

        public DateTime? SyncLast { get; set; }

    }
}
