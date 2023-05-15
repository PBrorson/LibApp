using Library.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Database.Context
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
        {
        }


        public DbSet<Category> Categories { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            {
                modelBuilder.Entity<Category>().HasData(new Category
                {
                    Id =1,
                    CategoryName = "Comedy",
                });
                modelBuilder.Entity<Category>().HasData(new Category
                {
                    Id=2,
                    CategoryName = "Sci-fi",
                });
                modelBuilder.Entity<Category>().HasData(new Category
                {
                    Id=3,
                    CategoryName = "Drama",
                });
                modelBuilder.Entity<Category>().HasData(new Category
                {
                    Id=4,
                    CategoryName = "Spy",
                });

            }


        }

    }
}