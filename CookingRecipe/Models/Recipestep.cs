using System;
using System.Collections.Generic;

namespace CookingRecipe.Models;

public partial class Recipestep
{
    public int StepId { get; set; }

    public int RecipeId { get; set; }

    public int StepNumber { get; set; }

    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    /// <summary>
    /// Thời gian của bước này (phút)
    /// </summary>
    public int? Duration { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;
}
