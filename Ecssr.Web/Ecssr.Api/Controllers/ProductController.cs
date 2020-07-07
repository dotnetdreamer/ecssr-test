using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Ecssr.Api.Dto;
using Ecssr.Core.Domain;
using Ecssr.Services.Catalog;
using Ecssr.Web.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;

namespace Ecssr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IElasticClient _elasticClient;
        private readonly IMapper _mapper;

        public ProductController(IElasticClient elasticClient
            , IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _elasticClient = elasticClient;
            _mapper = mapper;
        }

        [HttpGet("get")]
        public string Get()
        {
            return "Api is running";
        }

        [HttpGet("dashboardReport")]
        public async Task<IActionResult> DashboardReport()
        {
            var totalIndexed = await _elasticClient.CountAsync<Product>();
            var totalProducts = _productService.GetProductList(pageIndex: 0, pageSize: 1).TotalCount;

            return Ok(new
            {
                totalIndexed,
                totalProducts
            });
        }

        [HttpGet("getProductList")]
        public IActionResult GetProductList(int pageIndex = 0, int pageSize = 5)
        {
            var products = _productService.GetProductList(
                pageIndex: pageIndex, pageSize: pageSize);

            var model = new DataSourceResult
            {
                Data = products
                .Select(x =>
                {
                    var dto = _mapper.Map<ProductDto>(x);
                    return dto;
                }),
                Total = products.TotalCount
            };

            return Ok(model);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string term, int pageIndex = 1, int pageSize = 5)
        {
            var response = await _elasticClient.SearchAsync<Product>(
                 s => s.Query(q => q.QueryString(d => d.Query($"*{term}*")))
                     .From((pageIndex - 1) * pageSize)
                     .Size(pageSize));

            if (!response.IsValid)
                return Ok(new Product[] { });

            return Ok(response.Documents);
        }

    }
}
