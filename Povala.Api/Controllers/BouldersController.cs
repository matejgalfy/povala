using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Api.Models;
using System.Security.Claims;
using Povala.Shared.Constants;

namespace Povala.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BouldersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BouldersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Boulder>> GetBoulder(int id)
        {
            var result = await _context.Boulders.FirstOrDefaultAsync(b => b.Id == id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        // Maybe change it to some boulderDTO later
        // pagination maybe needed later if there is a lot of boulders
        public async Task<ActionResult<List<Boulder>>> GetAllBoulders()
        {
            var result = await _context.Boulders.Where(b => b.IsActive == true).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        // maybe change type of input parameter to CreateBoulderDTO later
        public async Task<ActionResult<Boulder>> CreateBoulder(Boulder boulder) 
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            _context.Boulders.Add(boulder);
            await _context.SaveChangesAsync();
            return Ok(boulder); 
        }


        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateBoulder(int id, Boulder boulder)
        {
            if (id != boulder.Id)
                return BadRequest("ID in URL and ID in body do not match.");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            _context.Entry(boulder).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Boulders.AnyAsync(e => e.Id == boulder.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteBoulder(int id)
        {
            var boulder = await _context.Boulders.FirstOrDefaultAsync(b => b.Id == id);
            if (boulder == null)
                return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            boulder.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
