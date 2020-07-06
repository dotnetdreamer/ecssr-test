using Ecssr.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecssr.Services.Catalog
{
    public interface IProductService
    {
        Task DeleteAsync(Product product);
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetProductList(int page, int skip = 0);
        Task<IEnumerable<Product>> GetProductsByCategory(string category);
        Task SaveBulkAsync(Product[] products);
        Task SaveManyAsync(Product[] products);
        Task SaveSingleAsync(Product product);
    }
}