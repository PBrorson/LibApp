using Library.Database.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Database.Models
{
    public class LibraryItem 
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Pages must be a positive number")]
        public int? Pages { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "RunTimeMinutes must be a positive number")]
        public int? RunTimeMinutes { get; set; } = 0;

        public bool IsBorrowable { get; set; } 

        [StringLength(50, ErrorMessage = "Borrower length cannot exceed 100 characters")]
        public string Borrower { get; set; }

        public DateTime? BorrowDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public LibraryItemType Type { get; set; }
        
        public string TitleAcronym { get; set; }

        public decimal FinePerDay { get; set; }
       
    }
}
