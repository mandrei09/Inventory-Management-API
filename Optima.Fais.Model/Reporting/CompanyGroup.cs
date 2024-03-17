﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class CompanyGroup
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Month { get; set; }
		public decimal Value { get; set; }
		public decimal ValueDep { get; set; }
	}

}