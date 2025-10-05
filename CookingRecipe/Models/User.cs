using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingRecipe.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public DateTime? CreatedAt { get; set; }
    [Column("role_id")]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role? Role { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
