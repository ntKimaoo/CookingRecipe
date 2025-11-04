using CookingRecipe.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static CookingRecipe.DTOs.RecipeDTOs;

namespace CookingRecipe.Controllers;

/// <summary>
/// Controller for managing recipes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecipeController"/> class.
    /// </summary>
    /// <param name="recipeService">The recipe service.</param>
    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Gets all recipes.
    /// </summary>
    /// <returns>A list of recipes.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeListDto>>> GetAll()
    {
        var recipes = await _recipeService.GetAllRecipesAsync();
        return Ok(recipes);
    }
    [HttpGet("recommend")]
    public async Task<ActionResult<IEnumerable<RecipeListDto>>> GetListRecipeRecommend()
    {
        var recipes = await _recipeService.GetRecommendRecipeAsync();
        return Ok(recipes);
    }
    
    /// <summary>
    /// Gets a recipe by its ID.
    /// </summary>
    /// <param name="id">The recipe ID.</param>
    /// <returns>The recipe details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetById(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);
        if (recipe == null)
            return NotFound(new { message = "Recipe not found" });

        return Ok(recipe);
    }

    /// <summary>
    /// Gets recipes by author ID.
    /// </summary>
    /// <param name="authorId">The author ID.</param>
    /// <returns>A list of recipes by the author.</returns>
    [HttpGet("author/{authorId}")]
    public async Task<ActionResult<IEnumerable<RecipeListDto>>> GetByAuthor(int authorId)
    {
        var recipes = await _recipeService.GetRecipesByAuthorAsync(authorId);
        return Ok(recipes);
    }

    /// <summary>
    /// Gets recipes by category ID.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>A list of recipes in the category.</returns>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<RecipeListDto>>> GetByCategory(int categoryId)
    {
        var recipes = await _recipeService.GetRecipesByCategoryAsync(categoryId);
        return Ok(recipes);
    }

    /// <summary>
    /// Searches recipes by keyword.
    /// </summary>
    /// <param name="keyword">The search keyword.</param>
    /// <returns>A list of recipes matching the keyword.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RecipeListDto>>> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest(new { message = "Keyword is required" });

        var recipes = await _recipeService.SearchRecipesAsync(keyword);
        return Ok(recipes);
    }

    /// <summary>
    /// Creates a new recipe.
    /// </summary>
    /// <param name="dto">The recipe creation DTO.</param>
    /// <returns>The created recipe.</returns>
    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Create([FromBody] CreateRecipeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _recipeService.CreateRecipeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.RecipeId }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing recipe.
    /// </summary>
    /// <param name="id">The recipe ID.</param>
    /// <param name="dto">The recipe update DTO.</param>
    /// <returns>The updated recipe.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<RecipeDto>> Update(int id, [FromBody] UpdateRecipeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _recipeService.UpdateRecipeAsync(id,dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a recipe by its ID.
    /// </summary>
    /// <param name="id">The recipe ID.</param>
    /// <returns>No content if deleted, or not found if the recipe does not exist.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _recipeService.DeleteRecipeAsync(id);
        if (!result)
            return NotFound(new { message = "Recipe not found" });

        return NoContent();
    }
}
