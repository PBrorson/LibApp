using Library.Database.Context;
using Library.Database.Models;
using Library.Services.Interface.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Services
{
    public class CategoryService : ICategoryService

    {
        private readonly ILogger<CategoryService> _logger;
        private readonly LibraryContext _context;

        public CategoryService(LibraryContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public ILogger Logger => _logger;

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                _logger.LogError("Category name cannot be null or empty.");
                return null;
            }

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);
            if (existingCategory != null)
            {
                _logger.LogError($"Category with name: {category.CategoryName} already exists.");
                return null;
            }

            _logger.LogInformation($"Creating a new category with name: {category.CategoryName}");
            var entry = await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Category with name: {category.CategoryName} created successfully");

            return entry.Entity;
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation($"Deleting category with ID: {id}");
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Category with ID: {id} deleted successfully");
            return category;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("No categories found.");
                return null;
            }

            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid ID.");
                return null;
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning($"No category found with ID: {id}.");
            }

            return category;
        }

        public async Task<bool> IsCategoryReferencedAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid ID.");
                return false;
            }

            var isReferenced = await _context.LibraryItems.AnyAsync(li => li.CategoryId == id);

            if (!isReferenced)
            {
                _logger.LogWarning($"No references found for category ID: {id}.");
            }

            return isReferenced;
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category updatedCategory)
        {
            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return null; //
            }
            existingCategory.CategoryName = updatedCategory.CategoryName;
         
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return null; 
                }
                else
                {
                    throw;
                }
            }

            return existingCategory;
        }



        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

