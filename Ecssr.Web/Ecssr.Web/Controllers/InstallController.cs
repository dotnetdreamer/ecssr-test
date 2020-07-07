using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecssr.Web.Models;
using Ecssr.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Ecssr.Core.Domain;
using Nest;
using Ecssr.Services.Catalog;

namespace Ecssr.Web.Controllers
{
    public class InstallController : Controller
    {
        private readonly ILogger<InstallController> _logger;
        private readonly EcssrDbContext _ecssrDbContext;
        private readonly IMapper _mapper;
        private readonly IElasticClient _elasticClient;
        private readonly IProductService _productService;

        public InstallController(ILogger<InstallController> logger
            , EcssrDbContext ecssrDbContext, IMapper mapper
            , IElasticClient elasticClient, IProductService productService)
        {
            _logger = logger;
            _ecssrDbContext = ecssrDbContext;
            _mapper = mapper;
            _elasticClient = elasticClient;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _ecssrDbContext.Products.CountAsync();
            if (products > 0)
                ViewBag.DbInstalled = true;

            var totalIndexed = await _elasticClient.CountAsync<Product>();
            ViewBag.TotalIndexed = totalIndexed.Count;

            return View();
        }

        public async Task<IActionResult> Run()
        {
            var products = await _ecssrDbContext.Products.CountAsync();
            if (products == 0)
            {
                var productsToAdd = new List<Product>();

                //seed data
                const int RECORDS_TO_INSERT = 10;
                for (int i = 0; i < RECORDS_TO_INSERT; i++)
                {
                    var model = new ProductViewModel
                    {
                        Name = $"Product - {i}",
                        Category = $"Category 1",
                        CompanyName = $"Company 1",
                        Color = "Red",
                        Description = $"Description goes here for product {i}",
                        Model = "Model Name",
                        Price = i * 1.5,
                        VideoUrl = "",
                    };

                    var product = _mapper.Map<Product>(model);
                    product.CreatedOn = DateTime.UtcNow.AddMinutes(i);

                    productsToAdd.Add(product);
                }

                await this._productService.AddManyAsync(productsToAdd.ToArray());
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RunIndexer()
        {
            var totalIndexed = await _elasticClient.CountAsync<Product>();
            if (totalIndexed.Count == 0)
            {
                await _elasticClient.DeleteByQueryAsync<Product>(q => q.MatchAll());

                var products = (await _productService.GetProductList(int.MaxValue)).ToArray();
                foreach (var product in products)
                {
                    await _elasticClient.IndexDocumentAsync(product);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
