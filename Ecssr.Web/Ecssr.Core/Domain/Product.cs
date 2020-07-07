using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecssr.Core.Domain
{
    [Table("Product")]
    public class Product : BaseEntity
    {
        private ICollection<ProductPicture> _productPictures;

        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Model { get; set; }
        public string VideoUrl { get; set; }
        public string CompanyName { get; set; }
        public string Category { get; set; }

        public virtual ICollection<ProductPicture> ProductPictures
        {
            get => _productPictures ?? (_productPictures = new List<ProductPicture>());
            protected set => _productPictures = value;
        }
    }
}
