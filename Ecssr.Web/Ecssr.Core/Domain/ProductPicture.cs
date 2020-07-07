using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecssr.Core.Domain
{
    [Table("ProductPicture")]
    public class ProductPicture
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
