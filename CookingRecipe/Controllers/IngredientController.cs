using CookingRecipe.DTOs;
using CookingRecipe.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CookingRecipe.Controllers
{
    /// <summary>
    /// API controller for managing ingredients.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientController"/> class.
        /// </summary>
        /// <param name="ingredientService">Service for ingredient operations.</param>
        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        /// <summary>
        /// Retrieves all ingredients.
        /// </summary>
        /// <returns>A list of ingredient DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAll()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            return Ok(ingredients);
        }

        /// <summary>
        /// Retrieves an ingredient by its ID.
        /// </summary>
        /// <param name="id">The ingredient ID.</param>
        /// <returns>The ingredient DTO if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientDto>> GetById(int id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
                return NotFound();
            return Ok(ingredient);
        }

        /// <summary>
        /// Creates a new ingredient.
        /// </summary>
        /// <param name="dto">The DTO containing ingredient data.</param>
        /// <returns>The created ingredient DTO.</returns>
        [HttpPost]
        public async Task<ActionResult<IngredientDto>> Create([FromBody] CreateIngredientDto dto)
        {
            var created = await _ingredientService.CreateIngredientAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.IngredientId }, created);
        }
                
        /// <summary>
        /// Updates an existing ingredient.
        /// </summary>
        /// <param name="id">The ingredient ID.</param>
        /// <param name="dto">The DTO containing updated ingredient data.</param>
        /// <returns>The updated ingredient DTO if found; otherwise, NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IngredientDto>> Update(int id, [FromBody] UpdateIngredientDto dto)
        {
            var updated = await _ingredientService.UpdateIngredientAsync(id, dto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Deletes an ingredient by its ID.
        /// </summary>
        /// <param name="id">The ingredient ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _ingredientService.DeleteIngredientAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}