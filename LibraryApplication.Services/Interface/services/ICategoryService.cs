
using Library.Database.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Interface.services
{
    public interface ICategoryService
    {
        

        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int id, Category updatedCategory);
        Task<Category> DeleteCategoryAsync(int id);
        Task<bool> IsCategoryReferencedAsync(int id);
    }
}
