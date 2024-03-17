using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
    public class AdministrationFAR
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal CurrBkValue { get; set; }
        public decimal AccumulDep { get; set; }
        public decimal CurrentAPC { get; set; }
        public decimal Procentage { get; set; }
    }
}
