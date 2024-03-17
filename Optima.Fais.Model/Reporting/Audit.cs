using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class Audit
    {
        [Key]
        public int Id { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string AdministrationCode { get; set; }
        public string AdministrationName { get; set; }
        public int Initial { get; set; }
        public decimal InitialValue { get; set; }
        public int Scanned { get; set; }
        public decimal ScannedValue { get; set; }
        public int NotScanned { get; set; }
        public decimal NotScannedValue { get; set; }
        public int FinalScanned { get; set; }
        public decimal FinalScannedValue { get; set; }
        public int Minus { get; set; }
        public decimal MinusValue { get; set; }
        public int Plus { get; set; }
        public decimal PlusValue { get; set; }
        public int Temporary { get; set; }
        public int TranInDivision { get; set; }
        public int TranInDivisionDIFFEMP { get; set; }
        public int TranInDivisionDIFFCC { get; set; }
        public int TranInDivisionDIFFRR { get; set; }
       
    }
}
