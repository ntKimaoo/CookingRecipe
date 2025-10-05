using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipe.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly CookingrecipeContext _context;
        public RoleRepository(CookingrecipeContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}
