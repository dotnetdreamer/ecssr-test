using Ecssr.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecssr.Services.Catalog
{
    public interface IProductService
    {
        Task DeleteAsync(Product product);
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetProductList(int pageIndex = 1, int skip = 0);
        Task AddManyAsync(Product[] products);
        Task AddAsync(Product product);
    }
}