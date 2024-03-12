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

            var recently_created_project = context.ProjectTasks.OrderByDescending(s=>s.ID)
            .Take(1)
            .FirstOrDefault();

            if(recently_created_project != null)
                await CreateDonationsHeaderFile(project, recently_created_project.ID); // persist
            
            return Ok(result);
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

    }
    async Task RemoveDonationsHeaderFile(int donationsId)
    {
        try
        {
            var beneficiary = context.DonationsHeaders.Where(i=>i.Id==donationsId).FirstOrDefault();
            
            if(beneficiary == null)
                return;
            context.DonationsHeaders.Remove(beneficiary);
            await context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
        }

    }
    async Task CreateDonationsHeaderFile(ProjectTasks project, int projectId)
    {
        try
        {
            var beneficiary = new DonationsHeader();
            beneficiary.ProjectId = projectId;
            beneficiary.Recipient = project.Caption;
            beneficiary.Period = $"{project.DateStart.ToString("MMMM dd, yyyy")}";
            context.DonationsHeaders.Add(beneficiary);
            await context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            throw ex;
        }

    }

    [HttpPut("Project/UpdateProject/{id}")]
    public async Task<ActionResult> UpdateProject([FromRoute] int id, ProjectTasks project)
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

        var donationsId = context.DonationsHeaders
        .Where(i=>i.ProjectId == id)
        .FirstOrDefault(); // remove also in the donations header table

        if(donationsId != null)
            await RemoveDonationsHeaderFile(donationsId.Id);

        context.ProjectTasks.Remove(projectTasks);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProjectExists(int id)
    {
        return (context.ProjectTasks?.Any(i => i.ID == id)).GetValueOrDefault();
    }
}