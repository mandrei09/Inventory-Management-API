using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class RouteData
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public string Variant { get; set; }
        public BadgeBase Badge { get; set; }
        public bool Active { get; set; }
        public string Color { get; set; }
        public bool Title { get; set; }
        public bool Divider { get; set; }

        public IEnumerable<RouteChildrenBase> Children { get; set; }
    }
}
