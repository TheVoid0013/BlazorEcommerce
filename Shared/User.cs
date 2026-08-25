using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorEcommerce.Shared
{
    [Table("Users")]
    [Index(nameof(Email), IsUnique = true, Name = "IX_Users_Email_Unique")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = new byte[32];  // Default 256-bit hash

        [Required]
        public byte[] PasswordSalt { get; set; } = new byte[32];  // Default 256-bit salt

        [Required]
        public DateTime DataCreated { get; set; } = DateTime.Now;
    }
}