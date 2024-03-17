﻿using System;

namespace Optima.Fais.Dto
{
    public class CategoryEN
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CodeNameEntity InterCompany { get; set; }
        public CodeNameEntity InterCompanyEN { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}