using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class TableDefinitionData
    {
        public TableDefinitionBase TableDefinition { get; set; }
        public IEnumerable<ColumnDefinitionBase> ColumnDefinitions { get; set; }
    }
}
