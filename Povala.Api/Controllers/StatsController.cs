using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Povala.Api.Services;
using Povala.Shared.DTOs.User;
using System.Security.Claims;

namespace Povala.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }


        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<UserStatsDto>> GetMyStats()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            return Ok(await _statsService.GetUserStatsAsync(int.Parse(userIdStr)));
        }
    }
}
