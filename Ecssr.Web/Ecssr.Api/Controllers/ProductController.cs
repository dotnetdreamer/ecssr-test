using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecssr.Core.Domain;
using Ecssr.Services.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;

namespace Ecssr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly IElasticClient _elasticClient;

        public ProductController(IElasticClient elasticClient
            , IProductService productService)
        {
            _productService = productService;
            _elasticClient = elasticClient;
        }

        [HttpGet("get")]
        public string Get()
        {
            return "Api is running";
        }

        [HttpGet("find")]
        public async Task<IActionResult> Find(string query, int page = 1, int pageSize = 5)
        {
            var response = await _elasticClient.SearchAsync<Product>(
                 s => s.Query(q => q.QueryString(d => d.Query($"*{query}*")))
                     .From((page - 1) * pageSize)
                     .Size(pageSize));

            if (!response.IsValid)
                return Ok(new Product[] { });

            return Ok(response.Documents);
        }

    }
}
