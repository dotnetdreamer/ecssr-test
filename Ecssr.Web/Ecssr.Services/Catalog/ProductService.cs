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
        private List<Product> _cache = new List<Product>();

        private readonly EcssrDbContext _ecssrDbContext;
        private readonly IElasticClient _elasticClient;

        public ProductService(IElasticClient elasticClient
            , EcssrDbContext ecssrDbContext)
        {
            _elasticClient = elasticClient;
            _ecssrDbContext = ecssrDbContext;
        }

        public async Task<IEnumerable<Product>> GetProductList(int pageIndex = 1, int skip = 0)
        {
            var products = await _ecssrDbContext.Products
                .Skip(skip)
                .Take(pageIndex - 1)
                .ToListAsync();

            return products;
        }

        public virtual Task<Product> GetProductById(int id)
        {
            var product = _cache
              .FirstOrDefault(p => p.Id == id);

            return Task.FromResult(product);
        }

        public virtual Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            var products = _cache
              .Where(p => p.Category.Contains(category, StringComparison.CurrentCultureIgnoreCase));

            return Task.FromResult(products);
        }

        public async Task DeleteAsync(Product product)
        {
            await _elasticClient.DeleteAsync<Product>(product);

            if (_cache.Contains(product))
            {
                _cache.Remove(product);
            }
        }

        public async Task SaveSingleAsync(Product product)
        {
            if (_cache.Any(p => p.Id == product.Id))
            {
                await _elasticClient.UpdateAsync<Product>(product, u => u.Doc(product));
            }
            else
            {
                _cache.Add(product);
                await _elasticClient.IndexDocumentAsync<Product>(product);
            }
        }

        public async Task SaveManyAsync(Product[] products)
        {
            _cache.AddRange(products);

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

        public async Task SaveBulkAsync(Product[] products)
        {
            _cache.AddRange(products);

            var result = await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(products));
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
