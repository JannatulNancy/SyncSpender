using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncSpender.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        
        [Column(TypeName = "nvarchar(100)")]
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(100)")]
        public string? Note { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        // Foreign key for the user
       
    }
}
