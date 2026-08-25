using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorEcommerce.Shared
{
    [Table("Categories")]
    [Index(nameof(Name), IsUnique = true, Name = "IX_Categories_Name_Unique")]
    [Index(nameof(Url), IsUnique = true, Name = "IX_Categories_Url_Unique")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Url { get; set; } = string.Empty;
    }
}