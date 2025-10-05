using CookingRecipe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly CookingrecipeContext _context;

        public TestController(CookingrecipeContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipestep>>> GetAll()
        {
            return await _context.Set<Recipestep>().ToListAsync();
        }
    }
}
