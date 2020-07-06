using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecssr.Core.Domain
{
    [Table("Product")]
    public class Product : BaseEntity
    {
        //(Id, Name, color, Description, Price, Model Number, Create Date, Video, Company Name, Category).
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
