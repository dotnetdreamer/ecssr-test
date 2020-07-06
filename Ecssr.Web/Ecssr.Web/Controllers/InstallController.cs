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

namespace Ecssr.Web.Controllers
{
    public class InstallController : Controller
    {
        private readonly ILogger<InstallController> _logger;
        private readonly EcssrDbContext _ecssrDbContext;
        private readonly IMapper _mapper;

        public InstallController(ILogger<InstallController> logger
            , EcssrDbContext ecssrDbContext, IMapper mapper)
        {
            _logger = logger;
            _ecssrDbContext = ecssrDbContext;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _ecssrDbContext.Products.CountAsync();
            if(products == 0)
            {
                //seed data
                const int RECORDS_TO_INSERT = 10;
                for (int i = 0; i < RECORDS_TO_INSERT; i++)
                {
                    var model = new ProductViewModel();

                    var product = _mapper.Map<Product>(model);
                    
                }

            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
