using AutoMapper;
using CookingRecipe.Models;
using CookingRecipe.Repositories;
using CookingRecipe.Services.Interfaces;
using static CookingRecipe.DTOs.RecipeDTOs;

namespace CookingRecipe.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RecipeService(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeListDto>> GetAllRecipesAsync()
        {
            var recipes = await _recipeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RecipeListDto>>(recipes);
        }
        public async Task<IEnumerable<RecipeListDto>> GetRecommendRecipeAsync()
        {
            var recipes = await _recipeRepository.GetRecommendRecipeAsync();
            return _mapper.Map<IEnumerable<RecipeListDto>>(recipes);
        }

        public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            return recipe == null ? null : _mapper.Map<RecipeDto>(recipe);
        }

        public async Task<IEnumerable<RecipeListDto>> GetRecipesByAuthorAsync(int authorId)
        {
            var recipes = await _recipeRepository.GetByAuthorIdAsync(authorId);
            return _mapper.Map<IEnumerable<RecipeListDto>>(recipes);
        }

        public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto)
        {
            var recipe = _mapper.Map<Recipe>(dto);
            if (dto.Ingredients != null && dto.Ingredients.Any())
            {
                recipe.RecipeIngredients = dto.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity
                }).ToList();
            }
            if (dto.Steps != null && dto.Steps.Any())
            {
                recipe.Recipesteps = dto.Steps.Select(s => new Recipestep
                {
                    StepNumber = s.StepNumber,
                    Content = s.Content,
                    ImageUrl = s.ImageUrl,
                    VideoUrl = s.VideoUrl,
                    Duration = s.Duration
                }).ToList();
            }

            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                recipe.RecipeCategories = dto.CategoryIds.Select(cid => new RecipeCategory
                {
                    CategoryId = cid
                }).ToList();
            }
            var created = await _recipeRepository.CreateAsync(recipe);
            var result = await _recipeRepository.GetByIdAsync(created.RecipeId);
            return _mapper.Map<RecipeDto>(result!);
        }

        public async Task<RecipeDto> UpdateRecipeAsync(int id,UpdateRecipeDto dto)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe==null)
                throw new KeyNotFoundException($"Recipe with ID {id} not found");
            _mapper.Map(dto, recipe);
            if (dto.Ingredients != null)
            {
                recipe.RecipeIngredients.Clear();
                recipe.RecipeIngredients = dto.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity
                }).ToList();
            }

            // Cập nhật Steps
            if (dto.Steps != null)
            {
                recipe.Recipesteps.Clear();
                recipe.Recipesteps = dto.Steps.Select(s => new Recipestep
                {
                    StepNumber = s.StepNumber,
                    Content = s.Content,
                    ImageUrl = s.ImageUrl,
                    VideoUrl = s.VideoUrl,
                    Duration = s.Duration
                }).ToList();
            }

            // Cập nhật Categories
            if (dto.CategoryIds != null)
            {
                recipe.RecipeCategories.Clear();
                recipe.RecipeCategories = dto.CategoryIds.Select(cid => new RecipeCategory
                {
                    CategoryId = cid
                }).ToList();
            }

            await _recipeRepository.UpdateAsync(recipe);

            return _mapper.Map<RecipeDto>(recipe);
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            return await _recipeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RecipeListDto>> SearchRecipesAsync(string keyword)
        {
            var recipes = await _recipeRepository.SearchAsync(keyword);
            return _mapper.Map<IEnumerable<RecipeListDto>>(recipes);
        }

        public async Task<IEnumerable<RecipeListDto>> GetRecipesByCategoryAsync(int categoryId)
        {
            var recipes = await _recipeRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<RecipeListDto>>(recipes);
        }
    }
}

