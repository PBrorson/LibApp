using Library.Database.Models;
using Library.Database.Models.Enum;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Interface.services
{
    public interface ILibraryService
    {

        Task<LibraryItem> GetLibraryItemByTitleAsync(string title);
        Task<IEnumerable<LibraryItem>> GetLibraryItemsAsync();
        Task<LibraryItem> GetLibraryItemByIdAsync(int id);
        Task CreateLibraryItemAsync(LibraryItem item);
        Task UpdateLibraryItemAsync(LibraryItem item);
        Task DeleteLibraryItemAsync(int id);
        Task<LibraryItem> CheckOutLibraryItemAsync(int id, string borrower);
        Task<LibraryItem> CheckInLibraryItemAsync(int id);
        Task<IEnumerable<LibraryItem>> GetLibraryItemsByTypeAsync(LibraryItemType type);
        Task<IEnumerable<LibraryItem>> SearchLibraryItemsAsync(string searchTerm);
        Task<IEnumerable<LibraryItem>> GetOverdueLibraryItemsAsync();

    }
}
