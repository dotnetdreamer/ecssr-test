using Ecssr.Data;
using Ecssr.Services.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecssr.Web.Framework.Extensions
{
    public static class EcssrExtensions
    {
        public static void AddEcssrDbContextAndServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration["ConnectionString:DbConnection"];
            services.AddDbContext<EcssrDbContext>(options => options.UseSqlServer(connection));

            //services
            services.AddTransient<IProductService, ProductService>();
        }
    }
}
