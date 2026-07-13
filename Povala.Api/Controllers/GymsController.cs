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
    public class GymsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public GymsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private static readonly List<Gym> MockGyms =
        [
            new() { Id = 1, Name = "Povala" },
            new() { Id = 2, Name = "Brutalka" }
        ];

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gym>>> GetAllGyms()
        {
            var result = await _context.Gyms.ToListAsync();
            return Ok(result);
        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult<Gym>> GetGym(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
            if (gym == null)
            {
                return NotFound("Gym not found");
            }
            return Ok(gym);
        }


        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<Gym>> GetMyGym()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userIdStr);
            if (user == null || user.ManagedGymId == null)
            {
                return NotFound("The current user doesn't own any gym");
            }

            var gym = await _context.Gyms.FindAsync(user.ManagedGymId);
            if (gym == null)
                return NotFound("The gym doesn't exist");
            return Ok(gym);
        }


        [HttpGet("{id}/boulders")]
        public async Task<ActionResult<IEnumerable<Boulder>>> GetBouldersForGym(int id)
        {
            if (!await _context.Gyms.AnyAsync(g => g.Id == id))
            {
                return NotFound("The gym doesn't exist");
            }

            var boulders = await _context.Boulders
                .Where(b => b.GymId == id)
                .ToListAsync();
            return Ok(boulders);
        }


        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<Gym>> CreateGym(Gym gym)
        {
            _context.Gyms.Add(gym);
            await _context.SaveChangesAsync();
            //return Ok(gym); 
            return CreatedAtAction(nameof(GetGym), new { id = gym.Id }, gym); 
            // use code above if it refuses to compile
        }


        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.GymOwner)]
        public async Task<IActionResult> UpdateGym(int id, Gym gym)
        {
            if (id != gym.Id)
            {
                return BadRequest("Id's don't match");
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();
            var user = await _userManager.FindByIdAsync(userIdStr);

            bool isAdmin = user?.Role == AppRoles.Admin;
            bool isOwnerOfThisGym = user?.ManagedGymId == id;
            if (!isAdmin && !isOwnerOfThisGym)
            {
                return Forbid();
            }

            _context.Entry(gym).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Gyms.AnyAsync(e => e.Id == id))
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
    }
}
