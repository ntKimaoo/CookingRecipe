using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public int RecipeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
