using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class RecipeIngredient
{
    public int RecipeIngredientId { get; set; }

    public int RecipeId { get; set; }

    public int IngredientId { get; set; }

    /// <summary>
    /// vd: 200g, 2 muỗng canh
    /// </summary>
    public string Quantity { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}
