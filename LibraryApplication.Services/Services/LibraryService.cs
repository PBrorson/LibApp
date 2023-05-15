using Library.Database.Context;
using Library.Database.Models;
using Library.Database.Models.Enum;
using Library.Services.Interface.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Services
{
    public class LibraryService :ILibraryService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LibraryService> _logger;
        private readonly LibraryContext _context;

        public LibraryService(LibraryContext context, ILogger<LibraryService> logger,IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }
        public ILogger Logger => _logger;

        public async Task<IEnumerable<LibraryItem>> GetLibraryItemsAsync()
        {
            return await _context.LibraryItems.Include(li => li.Category).ToListAsync();
        }

        public async Task<LibraryItem> GetLibraryItemByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching library item with ID: {id}");
            var item = await _context.LibraryItems.Include(li => li.Category).FirstOrDefaultAsync(li => li.Id == id);

            if (item == null)
            {
                _logger.LogWarning($"Library item with ID: {id} not found");
            }

            return item;
        }

        public async Task CreateLibraryItemAsync(LibraryItem item)
        {
            _logger.LogInformation($"Creating a new library item with title: {item.Title}");

            // Om samma titel finns
            var existingItem = await _context.LibraryItems
                .FirstOrDefaultAsync(li => li.Title.ToLower() == item.Title.ToLower());

            if (existingItem != null)
            {
                _logger.LogError($"Library item with title: {item.Title} already exists");
                throw new InvalidOperationException($"Library item with title: {item.Title} already exists");
            }

            item.TitleAcronym = GenerateTitleAcronym(item.Title);
            item.DueDate = CalculateDueDate();
            //sätter att ett item är inte uthyrbart när det skapats.
            item.IsBorrowable = false;
            item.FinePerDay = 0;
            _context.LibraryItems.Add(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Library item with title: {item.Title} created successfully");
        }


        public async Task UpdateLibraryItemAsync(LibraryItem item)
        {
            _logger.LogInformation($"Updating library item with ID: {item.Id}");

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Library item with ID: {item.Id} updated successfully");
        }

        public async Task DeleteLibraryItemAsync(int id)
        {
            _logger.LogInformation($"Deleting library item with ID: {id}");
            var item = await _context.LibraryItems.FindAsync(id);
            if (item != null)
            {
                _context.LibraryItems.Remove(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Library item with ID: {id} deleted successfully");
            }
        }

        public async Task<LibraryItem> CheckOutLibraryItemAsync(int id, string borrower)
        {
            var item = await _context.LibraryItems.FindAsync(id);
            if (item != null && item.Type != LibraryItemType.ReferenceBook && !item.IsBorrowable)
            {
                item.Borrower = borrower;
                item.BorrowDate = DateTime.UtcNow;
                item.DueDate = CalculateDueDate();
                item.IsBorrowable = true;

                await _context.SaveChangesAsync();
            }
            else if (item != null && item.Type == LibraryItemType.ReferenceBook)
            {
               
                _logger.LogWarning("Reference books cannot be borrowed.");
            }
            return item;
        }
        public async Task<LibraryItem> GetLibraryItemByTitleAsync(string title)
        {
            _logger.LogInformation($"Fetching library item with title: {title}");
            var item = await _context.LibraryItems.Include(li => li.Category).FirstOrDefaultAsync(li => li.Title == title);

            if (item == null)
            {
                _logger.LogWarning($"Library item with title: {title} not found");
            }

            return item;
        }


        public async Task<LibraryItem> CheckInLibraryItemAsync(int id)
        {
            var item = await _context.LibraryItems.FindAsync(id);
            if (item != null && item.IsBorrowable)
            {
                //uträkning för förseningsavgift
                if (item.DueDate < DateTime.UtcNow)
                {
                    var daysOverdue = (DateTime.UtcNow - item.DueDate.Value).Days;
                    var fine = daysOverdue * item.FinePerDay;
                    _logger.LogInformation($"Fine for library item is: {fine}");
                }

                item.Borrower = null;
                item.BorrowDate = null;
                item.IsBorrowable = false;
                item.DueDate = null;

                await _context.SaveChangesAsync();
            }
            return item;
        }

        public async Task<IEnumerable<LibraryItem>> GetLibraryItemsByTypeAsync(LibraryItemType type)
        {
            return await _context.LibraryItems
                .Include(li => li.Category)
                .Where(li => li.Type == type)
                .ToListAsync();
        }
        // min metod för att generera akronym på titlarna
        private string GenerateTitleAcronym(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            var acronym = string.Concat(title.Split(' ').Select(word => word[0])).ToUpper();
            return acronym;
        }

        //mest en kul grej att kunna söka på en titel eller författare. 
        public async Task<IEnumerable<LibraryItem>> SearchLibraryItemsAsync(string searchTerm)
        {
            _logger.LogInformation($"Searching library items with search term: {searchTerm}");

            var result = await _context.LibraryItems
                .Include(li => li.Category)
                .Where(li => li.Title.Contains(searchTerm) || li.Author.Contains(searchTerm))
                .ToListAsync();

            _logger.LogInformation($"Found {result.Count} items with search term: {searchTerm}");

            return result;
        }
        // ett kul försök här också, där man lägger till 14 dagar på uthyrning
        private DateTime? CalculateDueDate(DateTime? borrowDate)
        {
        
            if (borrowDate.HasValue)
            {
                return borrowDate.Value.AddDays(14); 
            }

            return null; 
        }

        // La till dessa för att kunna kalkylera uthyrning, men också för avgift efter Duedate
        private DateTime CalculateDueDate()
        {
            return DateTime.UtcNow.AddDays(14);
        }
        private decimal GetFinePerDay()
        {
            decimal finePerDay = _configuration.GetValue<decimal>("LibrarySettings:FinePerDay");
            return finePerDay;
        }

        public async Task<IEnumerable<LibraryItem>> GetOverdueLibraryItemsAsync()
        {
            _logger.LogInformation("Fetching overdue library items");

            var overdueItems = await _context.LibraryItems
                .Include(li => li.Category)
                .Where(li => li.IsBorrowable && li.DueDate < DateTime.UtcNow)
                .ToListAsync();

            if (!overdueItems.Any())
            {
                _logger.LogInformation("No overdue library items found");
            }

            return overdueItems;
        }
    }
}
