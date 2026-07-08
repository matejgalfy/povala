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
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var result = await _context.Projects.FirstOrDefaultAsync(b => b.Id == id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPost]
        // maybe change type of input parameter to CreateProjectDTO later
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return Ok(project);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(b => b.Id == id);
            if (project == null)
                return NotFound();

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || int.Parse(userId) != project.UserId)
                return Forbid();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
