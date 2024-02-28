using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
public class ProjectsManagerController : ControllerBase
{
    readonly RhfDbContext context;
    public ProjectsManagerController(RhfDbContext context)
    {
        this.context = context;
    }

    [HttpGet]
    [Route("GetProjects")]
    public async Task<ActionResult<IEnumerable<ProjectTasks>>> GetAllPlannedTasks()
    {
        if (context.ProjectTasks == null)
            return NotFound();

        return await context.ProjectTasks.ToListAsync();
    }

    [HttpPost]
    [Route("AddProject")]
    public async Task<ActionResult> PostProject([FromBody] ProjectTasks project)
    {
        try
        {
            if (project == null)
                return NotFound();

            context.ProjectTasks.Add(project);
            var result = await context.SaveChangesAsync();
            return Ok(result);
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

    }

    [HttpPut("Project/UpdateProject/{id}")]
    public async Task<ActionResult> UpdateProject([FromRoute]int id, ProjectTasks project)
    {
        if (project == null || project.ID != id)
            return BadRequest($"Invalid input data");

        var existingProject = await context.ProjectTasks.FindAsync(id);

        if (existingProject == null)
            return NotFound();

        try
        {
            context.Entry(existingProject).CurrentValues.SetValues(project);
            await context.SaveChangesAsync();
            return Ok();
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        if (!context.ProjectTasks.Any())
            return NotFound();

        var projectTasks = await context.ProjectTasks.FindAsync(id);

        if (projectTasks == null)
            return NotFound();

        context.ProjectTasks.Remove(projectTasks);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProjectExists(int id)
    {
        return (context.ProjectTasks?.Any(i => i.ID == id)).GetValueOrDefault();
    }
}