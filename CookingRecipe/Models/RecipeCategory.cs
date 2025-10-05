using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class RecipeCategory
{
    public int RecipeCategoryId { get; set; }

    public int RecipeId { get; set; }

    public int CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}
