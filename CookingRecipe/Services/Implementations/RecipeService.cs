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
            var created = await _recipeRepository.CreateAsync(recipe);
            var result = await _recipeRepository.GetByIdAsync(created.RecipeId);
            return _mapper.Map<RecipeDto>(result!);
        }

        public async Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto dto)
        {
            var exists = await _recipeRepository.ExistsAsync(dto.RecipeId);
            if (!exists)
                throw new KeyNotFoundException($"Recipe with ID {dto.RecipeId} not found");

            var recipe = _mapper.Map<Recipe>(dto);
            await _recipeRepository.UpdateAsync(recipe);
            var updated = await _recipeRepository.GetByIdAsync(dto.RecipeId);
            return _mapper.Map<RecipeDto>(updated!);
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

