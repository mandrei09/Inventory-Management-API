using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class RequestListKanban
    {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public string Priority { get; set; }
		public int ListPosition { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string Color { get; set; }
		public List<Reporter> Reporter { get; set; }
		public List<Reporter> Assignees { get; set; }
		public int? ProjectId { get; set; }
	}

	public class Reporter
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string AvatarUrl { get; set; }
	}
}
