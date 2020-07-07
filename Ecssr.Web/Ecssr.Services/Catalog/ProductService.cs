using Ecssr.Core;
using Ecssr.Core.Domain;
using Ecssr.Data;
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

        public IPagedList<Product> GetProductList(string term = ""
            , string color = "", DateTime? fromDate = null, DateTime? toDate = null
            , decimal? priceFrom = null, decimal? priceTo = null
            , int pageIndex = 0, int pageSize = int.MaxValue)
        {
            IQueryable<Product> query = _ecssrDbContext.Products
                .Include(p => p.ProductPictures);

            if (!string.IsNullOrEmpty(term))
                query = query.Where(p => p.Name.Contains(term));

            if (!string.IsNullOrEmpty(color))
                query = query.Where(p => p.Color.Contains(color));

            if (priceFrom != null)
                query = query.Where(p => p.Price >= priceFrom);

            if (priceTo != null)
                query = query.Where(p => p.Price <= priceTo);

            if (fromDate != null)
                query = query.Where(p => p.CreatedOn >= fromDate);

            if (toDate != null)
                query = query.Where(p => p.CreatedOn <= toDate);

            query = query.OrderByDescending(b => b.UpdatedOn ?? b.CreatedOn);

            //paging
            return new PagedList<Product>(query, pageIndex, pageSize);
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _ecssrDbContext.Products
              .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        }

        public async Task<int> GetProductPicturesCount()
        {
            var pictures = await _ecssrDbContext.ProductPictures.CountAsync();
            return pictures;
        }

        public async Task DeleteAsync(Product product)
        {
            await _elasticClient.DeleteAsync<Product>(product);

            var toRemove = await this.GetProductById(product.Id);
            if (toRemove != null)
            {
                _ecssrDbContext.Remove(toRemove);
                await _ecssrDbContext.SaveChangesAsync();
            }

        }

        public async Task AddAsync(Product product)
        {
            var toAddOrUpdate = await this.GetProductById(product.Id);
            if (toAddOrUpdate != null)
            {
                await _elasticClient.UpdateAsync<Product>(product, u => u.Doc(product));
                _ecssrDbContext.Update(toAddOrUpdate);
            }
            else
            {
                _ecssrDbContext.Add(toAddOrUpdate);
                await _elasticClient.IndexDocumentAsync<Product>(product);
            }

            await _ecssrDbContext.SaveChangesAsync();
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
