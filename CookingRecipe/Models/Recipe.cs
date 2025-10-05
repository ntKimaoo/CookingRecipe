using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Instructions { get; set; } = null!;

    /// <summary>
    /// Thời gian chuẩn bị (phút)
    /// </summary>
    public int? PrepTime { get; set; }

    /// <summary>
    /// Thời gian nấu (phút)
    /// </summary>
    public int? CookTime { get; set; }

    public string? Difficulty { get; set; }

    public int AuthorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    public virtual ICollection<Recipestep> Recipesteps { get; set; } = new List<Recipestep>();
}
