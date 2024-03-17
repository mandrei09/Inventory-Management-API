using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
    public partial class Address : Entity
    {
        public Address()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string UniqueName { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string AddressDetail { get; set; }

        public string PostalCode { get; set; }

        public int? CityId { get; set; }

        public City City { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

    }
}
