using Library.Database.Models;
using Library.Database.Models.Enum;
using Library.Services.Interface.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryItemsController : ControllerBase
    {
        private readonly ILogger<LibraryItemsController> _logger;
        private readonly ILibraryService _libraryService;

        public LibraryItemsController(ILibraryService libraryService, ILogger<LibraryItemsController> logger)
        {
            _libraryService = libraryService;
            _logger = logger;
        }

        [HttpGet("/GetAllItems")]
        public async Task<ActionResult<IEnumerable<LibraryItem>>> GetLibraryItems()
        {
            var items = await _libraryService.GetLibraryItemsAsync();
            return Ok(items);
        }

        [HttpGet("GetItemsById/{id}")]
        public async Task<ActionResult<LibraryItem>> GetLibraryItem(int id)
        {
            var item = await _libraryService.GetLibraryItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost("/CreateItems")]
        public async Task<ActionResult> CreateLibraryItem([FromBody] LibraryItem item)
        {
            if (item.Category == null || string.IsNullOrEmpty(item.Category.CategoryName))
            {
                ModelState.AddModelError("Category.CategoryName", "The CategoryName field is required.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _libraryService.CreateLibraryItemAsync(item);
            return CreatedAtAction(nameof(GetLibraryItem), new { id = item.Id }, item);
        }

        [HttpPut("UpdateItemsById/{id}")]
        public async Task<ActionResult> UpdateLibraryItem(int id, [FromBody] LibraryItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            if (item.Category == null || string.IsNullOrEmpty(item.Category.CategoryName))
            {
                ModelState.AddModelError("Category.CategoryName", "The CategoryName field is required.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _libraryService.UpdateLibraryItemAsync(item);


            _logger.LogInformation($"Updating library item with ID: {id}");

            var existingItem = await _libraryService.GetLibraryItemByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.CategoryId = item.CategoryId;
            existingItem.Title = item.Title;
            existingItem.Author = item.Author;
            existingItem.Pages = item.Pages;
            existingItem.RunTimeMinutes = item.RunTimeMinutes;
            existingItem.IsBorrowable = item.IsBorrowable;
            existingItem.Borrower = item.Borrower;
            existingItem.BorrowDate = item.BorrowDate;
            existingItem.DueDate = item.DueDate;
            existingItem.Type = item.Type;
            existingItem.TitleAcronym = item.TitleAcronym;
            existingItem.FinePerDay = item.FinePerDay; 

            await _libraryService.UpdateLibraryItemAsync(existingItem);
            _logger.LogInformation($"Library item with ID: {id} updated successfully");

            return NoContent();
        }

        [HttpDelete("DeleteItemById/{id}")]
        public async Task<ActionResult> DeleteLibraryItem(int id)
        {
            await _libraryService.DeleteLibraryItemAsync(id);
            return NoContent();
        }

        [HttpPost("items/{id}/checkout")]
        public async Task<ActionResult<LibraryItem>> CheckOutLibraryItem(int id, [FromBody] string borrower)
        {
            if (string.IsNullOrEmpty(borrower))
            {
                ModelState.AddModelError(nameof(borrower), "Borrower name is required.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _libraryService.CheckOutLibraryItemAsync(id, borrower);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost("items/{id}/checkin")]
        public async Task<ActionResult<LibraryItem>> CheckInLibraryItem(int id)
        {
            var item = await _libraryService.CheckInLibraryItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetLibraryItemsByType(LibraryItemType type)
        {
            if (!Enum.IsDefined(typeof(LibraryItemType), type))
            {
                return BadRequest("Invalid library item type.");
            }

            var items = await _libraryService.GetLibraryItemsByTypeAsync(type);
            return Ok(items);
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<IActionResult> SearchLibraryItems(string searchTerm)
        {
            var items = await _libraryService.SearchLibraryItemsAsync(searchTerm);
            return Ok(items);
        }
        [HttpGet("GetItemByTitle/{title}")]
        public async Task<ActionResult<LibraryItem>> GetLibraryItemByTitle(string title)
        {
            var item = await _libraryService.GetLibraryItemByTitleAsync(title);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [HttpGet("GetOverDueItems")]
        public async Task<ActionResult<IEnumerable<LibraryItem>>> GetOverdueLibraryItems()
        {
            var items = await _libraryService.GetOverdueLibraryItemsAsync();
            if (!items.Any())
            {
                return NotFound();
            }
            return Ok(items);
        }



    }

}