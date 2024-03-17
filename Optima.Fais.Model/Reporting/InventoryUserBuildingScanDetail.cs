using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class InventoryUserBuildingScanDetail
    {
        [Key]
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string RegionName { get; set; }
        public DateTime? Data { get; set; }
        public string Hour { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
       
    }
}
