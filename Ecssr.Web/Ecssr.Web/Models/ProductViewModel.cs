using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecssr.Web.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Model { get; set; }
        public string VideoUrl { get; set; }
        public string CompanyName { get; set; }
        public string Category { get; set; }

    }
}
