using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class Brand : Entity
	{
		public Brand()
		{

		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? DictionaryItemId { get; set; }

		public virtual DictionaryItem DictionaryItem { get; set; }

		public string Tag1Pattern { get; set; }

		public string Tag2Pattern { get; set; }

		public string Tag3Pattern { get; set; }

		public string Tag4Pattern { get; set; }

		public string Tag5Pattern { get; set; }

		public string Serial1Pattern { get; set; }

		public string Serial2Pattern { get; set; }

		public string Serial3Pattern { get; set; }

		public string Serial4Pattern { get; set; }

		public string Serial5Pattern { get; set; }

		public string Imei1Pattern { get; set; }

		public string Imei2Pattern { get; set; }

		public string Imei3Pattern { get; set; }

		public string Imei4Pattern { get; set; }

		public string Imei5Pattern { get; set; }

		public string PhoneNumber1Pattern { get; set; }

		public string PhoneNumber2Pattern { get; set; }

		public string PhoneNumber3Pattern { get; set; }

		public string PhoneNumber4Pattern { get; set; }

		public string PhoneNumber5Pattern { get; set; }

	}
}
