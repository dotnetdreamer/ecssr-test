using Ecssr.Core;
using Ecssr.Core.Domain;
using Ecssr.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecssr.Services.Catalog
{
    public class ProductService : IProductService
    {
        private readonly EcssrDbContext _ecssrDbContext;
        private readonly IElasticClient _elasticClient;

        public ProductService(IElasticClient elasticClient
            , EcssrDbContext ecssrDbContext)
        {
            _elasticClient = elasticClient;
            _ecssrDbContext = ecssrDbContext;
        }

        public IPagedList<Product> GetProductList(string term = "", string category = ""
            , string color = "", DateTime? fromDate = null, DateTime? toDate = null
            , decimal? priceFrom = null, decimal? priceTo = null
            , int pageIndex = 0, int pageSize = int.MaxValue)
        {
            IQueryable<Product> query = _ecssrDbContext.Products
                .Include(p => p.ProductPictures);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(term))
                query = query.Where(p => p.Name.Contains(term));

            if (!string.IsNullOrEmpty(color))
                query = query.Where(p => p.Color.Contains(color));

            if (priceFrom != null)
                query = query.Where(p => p.Price >= priceFrom);

            if (priceTo != null)
                query = query.Where(p => p.Price <= priceTo);

            if (fromDate != null)
            {
                var ids = _ecssrDbContext.Products.FromSqlRaw("select * from Product where CAST(CreatedOn AS DATE) >= CAST(@fromDate AS DATE)"
                    , new SqlParameter("fromDate", fromDate))
                    .Select(p => p.Id);
                query = query.Where(p => ids.Contains(p.Id));
            }

            if (toDate != null)
            {
                var ids = _ecssrDbContext.Products.FromSqlRaw("select * from Product where CAST(CreatedOn AS DATE) <= CAST(@toDate AS DATE)"
                    , new SqlParameter("toDate", toDate))
                    .Select(p => p.Id);
                query = query.Where(p => ids.Contains(p.Id));

            }

            query = query.OrderByDescending(b => b.UpdatedOn ?? b.CreatedOn);

            //paging
            return new PagedList<Product>(query, pageIndex, pageSize);
        }

        public async Task<int> GetProductPicturesCount()
        {
            var pictures = await _ecssrDbContext.ProductPictures.CountAsync();
            return pictures;
        }
        public async Task<List<string>> GetAllCategories()
        {
            var categories = await _ecssrDbContext.Products
                .GroupBy(p => p.Category)
                .Select(p => p.Key)
                .ToListAsync();

            return categories;
        }

        public async Task AddManyAsync(Product[] products)
        {
            _ecssrDbContext.AddRange(products);
            await _ecssrDbContext.SaveChangesAsync();

            var result = await _elasticClient.IndexManyAsync(products);
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    var msg = $"Failed to index document {itemWithError.Id}: {itemWithError.Error}";
                    throw new Exception(msg);
                }
            }
        }
    }
}
