using CookingRecipe.Models;

namespace CookingRecipe.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName);
    }
}
