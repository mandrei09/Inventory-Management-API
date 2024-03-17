using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class MatrixData
    {
        public string Name { get; set; }
        public IEnumerable<MatrixChildrenBase> Children { get; set; }
    }
}
