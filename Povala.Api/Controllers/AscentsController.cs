using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Api.Models;
using Povala.Shared.DTOs.Ascent;
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
        [Authorize]
        public async Task<ActionResult<AscentDto>> GetAscent(int id)
        {
            var result = await _context.Ascents.FirstOrDefaultAsync(a => a.Id == id);

            if (result == null)
                return NotFound();

            return Ok(new AscentDto
            {
                Id = result.Id,
                BoulderId = result.BoulderId,
                UserId = result.UserId
            });
        }


        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<AscentDto>>> GetMyAscents()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            var userId = int.Parse(userIdStr);

            var result = await _context.Ascents
                .Where(a => a.UserId == userId)
                .Select(a => new AscentDto
                {
                    Id = a.Id,
                    BoulderId = a.BoulderId,
                    UserId = a.UserId
                })
                .ToListAsync();

            return Ok(result);
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AscentDto>> CreateAscent(CreateAscentDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            var ascent = new Ascent
            {
                BoulderId = dto.BoulderId,
                UserId = int.Parse(userIdStr)
            };

            _context.Ascents.Add(ascent);
            await _context.SaveChangesAsync();

            return Ok(new AscentDto
            {
                Id = ascent.Id,
                BoulderId = ascent.BoulderId,
                UserId = ascent.UserId
            });
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAscent(int id)
        {
            var ascent = await _context.Ascents.FirstOrDefaultAsync(a => a.Id == id);
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
