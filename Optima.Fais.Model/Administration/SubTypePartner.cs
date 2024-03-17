using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class SubTypePartner : Entity
    {
        public SubTypePartner()
        {
        }

        public int SubTypeId { get; set; }

        public SubType SubType { get; set; }

        public int PartnerId { get; set; }

        public Partner Partner { get; set; }


    }
}
