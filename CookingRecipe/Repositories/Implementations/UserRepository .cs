using CookingRecipe.Models;
using CookingRecipe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipe.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly CookingrecipeContext _context;

        public UserRepository(CookingrecipeContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Recipes)
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                                    .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
