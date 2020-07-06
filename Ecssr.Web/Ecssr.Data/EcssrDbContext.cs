using Ecssr.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecssr.Data
{
    public class EcssrDbContext: DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<ProductPicture> ProductPictures { get; set; }


        public EcssrDbContext(DbContextOptions<EcssrDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(a => a.Price).HasColumnType("decimal(18, 2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
