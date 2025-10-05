using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
