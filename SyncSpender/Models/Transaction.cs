using SyncSpender.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncSpender.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Amount cannot be 0.")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(100)")]
        public string? Note { get; set; }
        [Range(1,int.MaxValue, ErrorMessage ="Please select the category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        // Foreign key for the user
        [NotMapped]
        public string? CategoryTitleWithIcon => Category == null ? "" : Category.Icon + " " + Category.Title;

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                if (Category == null)
                    return "+" + Amount.ToString("C0"); // Treat uncategorized as income
                return (Category.Type == "Expense" ? "-" : "+") + Amount.ToString("C0");
            }
        }
    }
}

