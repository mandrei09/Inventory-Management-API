using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class RequestKanban
    {
		public int Id { get; set; }
		public string Status { get; set; }
		public string Color { get; set; }
		public List<Card> List { get; set; }
	}

	public class Card
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public string Like { get; set; }
		public List<Comment> Comments { get; set; }
	}

	public class Comment
	{
		public int Id { get; set; }
		public string Text { get; set; }
	}
}
