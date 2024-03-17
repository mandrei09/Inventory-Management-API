namespace Optima.Fais.Dto
{
    public class ColumnDefinitionBase
    {
        //public int Id { get; set; }
        //public int TableDefinitionId { get; set; }
        public string HeaderCode { get; set; }
        public string Property { get; set; }
        public string Include { get; set; }
        public string SortBy { get; set; }
        public string Pipe { get; set; }
        public string Format { get; set; }
        public string TextAlign { get; set; }
        public bool Active { get; set; }
        public int Position { get; set; }
    }
}
