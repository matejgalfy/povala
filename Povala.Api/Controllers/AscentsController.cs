using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Api.Models;
using System.Security.Claims;

namespace Povala.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AscentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AscentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ascent>> GetAscent(int id)
        {
            var result = await _context.Ascents.FirstOrDefaultAsync(b => b.Id == id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPost]
        // maybe change type of input parameter to CreateAscentDTO later
        public async Task<ActionResult<Ascent>> CreateAscent(Ascent ascent)
        {
            _context.Ascents.Add(ascent);
            await _context.SaveChangesAsync();
            return Ok(ascent);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAscent(int id)
        {
            var ascent = await _context.Ascents.FirstOrDefaultAsync(b => b.Id == id);
            if (ascent == null)
                return NotFound();

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || int.Parse(userId) != ascent.UserId)
                return Forbid();

            _context.Ascents.Remove(ascent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
