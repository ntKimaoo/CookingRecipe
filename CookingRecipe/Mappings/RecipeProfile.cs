using AutoMapper;
using CookingRecipe.DTOs;
using CookingRecipe.Models;
using static CookingRecipe.DTOs.RecipeDTOs;

namespace CookingRecipe.Mappings
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            // Recipe mappings
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Recipesteps))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.RecipeCategories.Select(rc => rc.Category)))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.Favorites.Count));

            CreateMap<Recipe, RecipeListDto>()
                .ForMember(dest => dest.TotalTime, opt => opt.MapFrom(src => (src.PrepTime ?? 0) + (src.CookTime ?? 0)))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Username))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.Favorites.Count));

            CreateMap<CreateRecipeDto, Recipe>()
                .ForMember(dest => dest.RecipeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeCategories, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.Recipesteps, opt => opt.Ignore());

            CreateMap<UpdateRecipeDto, Recipe>()
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeCategories, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.Recipesteps, opt => opt.Ignore());

            // Related entities
            CreateMap<User, AuthorDto>();
            CreateMap<RecipeIngredient, RecipeIngredientDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Ingredient.Name))
                .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.IngredientId));
            CreateMap<Recipestep, RecipeStepDto>();
            CreateMap<Category, CategoryDto>();
        }
    }
}