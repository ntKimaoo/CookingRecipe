using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
}
