using Ecssr.Core;
using Ecssr.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecssr.Services.Catalog
{
    public interface IProductService
    {
        Task DeleteAsync(Product product);
        Task<Product> GetProductById(int id);

        IPagedList<Product> GetProductList(string term = ""
            , string color = "", DateTime? fromDate = null, DateTime? toDate = null
            , decimal? priceFrom = null, decimal? priceTo = null
            , int pageIndex = 0, int pageSize = int.MaxValue);

        Task<int> GetProductPicturesCount();

        Task AddManyAsync(Product[] products);
        Task AddAsync(Product product);
    }
}