using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class RoomSync
	{
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ErpCode { get; set; }
        public int? LocationId { get; set; }
        public bool IsFinished { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
