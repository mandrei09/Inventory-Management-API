using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class EntityFilePartner : Entity
    {
        public EntityFilePartner()
        {
        }

        public int EntityFileId { get; set; }

        public EntityFile EntityFile { get; set; }

        public int PartnerId { get; set; }

        public Partner Partner { get; set; }

		public Guid Guid { get; set; }

		public bool Selected { get; set; }

	}
}
