using Ecssr.Core;
using Ecssr.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecssr.Services.Catalog
{
    public interface IProductService
    {
        Task DeleteAsync(Product product);
        Task<Product> GetProductById(int id);
        IPagedList<Product> GetProductList(int pageIndex = 0, int pageSize = int.MaxValue);
        Task AddManyAsync(Product[] products);
        Task AddAsync(Product product);
    }
}