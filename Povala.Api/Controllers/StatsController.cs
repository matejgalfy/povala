using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Shared.DTOs.User;
using System.Security.Claims;

namespace Povala.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<UserStatsDto>> GetMyStats()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            var userId = int.Parse(userIdStr);

            // Ascents count even if the boulder was removed from the wall since
            var ascendedBoulders = await _context.Ascents
                .Where(a => a.UserId == userId)
                .Join(_context.Boulders, a => a.BoulderId, b => b.Id, (a, b) => new { b.Id, b.Difficulty })
                .Distinct()
                .ToListAsync();

            var ascendedIds = ascendedBoulders.Select(b => b.Id).ToHashSet();

            // Only projects on boulders that are still on the wall and not yet sent
            var projectBoulderIds = await _context.Projects
                .Where(p => p.UserId == userId)
                .Join(_context.Boulders.Where(b => b.IsActive), p => p.BoulderId, b => b.Id, (p, b) => b.Id)
                .Distinct()
                .ToListAsync();

            return Ok(new UserStatsDto
            {
                TotalAscents = ascendedBoulders.Count,
                ActiveProjects = projectBoulderIds.Count(id => !ascendedIds.Contains(id)),
                HighestGrade = ascendedBoulders.Count > 0 ? ascendedBoulders.Max(b => b.Difficulty) : null
            });
        }
    }
}
