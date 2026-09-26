using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Povala.Api.Data;
using Povala.Api.Models;
using Povala.Shared.DTOs.Project;
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
        [Authorize]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var result = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (result == null)
                return NotFound();

            return Ok(new ProjectDto
            {
                Id = result.Id,
                BoulderId = result.BoulderId,
                UserId = result.UserId
            });
        }


        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<ProjectDto>>> GetMyProjects()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            var userId = int.Parse(userIdStr);

            var result = await _context.Projects
                .Where(p => p.UserId == userId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    BoulderId = p.BoulderId,
                    UserId = p.UserId
                })
                .ToListAsync();

            return Ok(result);
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
                return Unauthorized();

            var project = new Project
            {
                BoulderId = dto.BoulderId,
                UserId = int.Parse(userIdStr)
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new ProjectDto
            {
                Id = project.Id,
                BoulderId = project.BoulderId,
                UserId = project.UserId
            });
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
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
