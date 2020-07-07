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
                var productNames = new List<string>()
                {
                    "Build your own computer",
                    "Digital Storm VANQUISH 3 Custom Performance PC", "Apple MacBook Pro 13-inch",
                    "Asus N551JK-XO076H Laptop", "Samsung Series 9 NP900X4C Premium Ultrabook",
                }.ToArray();
                var categoryNames = new List<string> { "Notebooks", "Laptops", "Desktops", "Mobiles","Cameras" }.ToArray();

                var productsToAdd = new List<Product>();
                //seed data
                const int RECORDS_TO_INSERT = 200;
                for (int i = 0; i < RECORDS_TO_INSERT; i++)
                {
                    var productRand = new Random().Next(0, 4);
                    var model = new ProductViewModel
                    {
                        Name = $"{productNames[productRand]}",
                        Category = $"{categoryNames[productRand]}",
                        CompanyName = $"Company 1",
                        Color = "Red",
                        Description = $"Description goes here for product {i}",
                        Model = $"Model Name for {i}",
                        Price = i * 2,
                        VideoUrl = ""
                    };

                    var product = _mapper.Map<Product>(model);
                    product.CreatedOn = DateTime.UtcNow.AddMinutes(i);

                    //picture 1
                    var pic1 = new Random().Next(1, 5);
                    var pic1FullPath = $"sample/{pic1}.jpeg";
                    product.ProductPictures.Add(new ProductPicture
                    {
                        Product = product,
                        ImageUrl = pic1FullPath
                    });

                    var pic2 = new Random().Next(1, 5);
                    var pic2FullPath = $"sample/{pic2}.jpeg";
                    product.ProductPictures.Add(new ProductPicture
                    {
                        Product = product,
                        ImageUrl = pic2FullPath
                    });


                    var pic3 = new Random().Next(1, 5);
                    var pic3FullPath = $"sample/{pic3}.jpeg";
                    product.ProductPictures.Add(new ProductPicture
                    {
                        Product = product,
                        ImageUrl = pic3FullPath
                    });


                    var pic4 = new Random().Next(1, 5);
                    var pic4FullPath = $"sample/{pic4}.jpeg";
                    product.ProductPictures.Add(new ProductPicture
                    {
                        Product = product,
                        ImageUrl = pic4FullPath
                    });

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

                var products = _productService.GetProductList().ToArray();
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


        #region Utillities

        #endregion
    }
}
