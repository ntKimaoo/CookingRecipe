using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingRecipe.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
