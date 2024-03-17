using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class RouteChildren : Entity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string? Href { get; set; }
        public string Icon { get; set; }
        public bool Title { get; set; }
        public bool Divider { get; set; }
        public string? Class { get; set; }
        public int? RouteId { get; set; }
        public virtual Route Route { get; set; }
        public string RoleId { get; set; }
        public virtual ApplicationRole Role { get; set; }
        public string Variant { get; set; }
        public bool Active { get; set; }
        public int? BadgeId { get; set; }
        public virtual Badge Badge { get; set; }
        public int? IconRouteId { get; set; }
        public virtual IconRoute IconRoute { get; set; }

        public int Position { get; set; }

        public Guid Guid { get; set; }

    }
}
