using Ecssr.Core;
using Ecssr.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecssr.Services.Catalog
{
    public interface IProductService
    {
        IPagedList<Product> GetProductList(string term = "", string category = ""
            , string color = "", DateTime? fromDate = null, DateTime? toDate = null
            , decimal? priceFrom = null, decimal? priceTo = null
            , int pageIndex = 0, int pageSize = int.MaxValue);

        Task<int> GetProductPicturesCount();

        Task<List<string>> GetAllCategories();
    }
}