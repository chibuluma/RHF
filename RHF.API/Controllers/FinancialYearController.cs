using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
public class FinancialYearController : ControllerBase
{
    private readonly RhfDbContext context;

    public FinancialYearController(RhfDbContext dbcontext)
    {
        context = dbcontext;
    }
    [HttpGet]
    [Route("GetFinancialYears")]
    public async Task<IActionResult> GetFinancialYears(){
        var result = await context.FinancialYear.OrderByDescending(d=>d.Year).ToListAsync();
        return Ok(result);
    }
    [HttpPost]
    [Route("PostFinancialYear")]
    public async Task<IActionResult> PostFinancialYear([FromBody] FinancialYear financialYear)
    {
        try
        {
            if (financialYear == null)
                return BadRequest("Financial year details can not be null");

            if(FinancialYearExists(financialYear.Year))
                return BadRequest($"Financial year {financialYear.Year} already exixts");

            await CheckIfCurrentFinancialYearExists(financialYear.IsCurrent);

            context.FinancialYear.Add(financialYear);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostFinancialYear), new { id = financialYear.Id }, financialYear);

        }
        catch (System.Exception ex)
        {
            return BadRequest($"Error {ex}");
        }
    }

    private async Task CheckIfCurrentFinancialYearExists(bool isCurrent)
    {
        if (isCurrent)
        {
            //check if any year has been set as current and revert
            var available_financial_years = context.FinancialYear
                    .Where(i => i.IsCurrent)
                    .ToList();

            if (available_financial_years.Count > 0)
                foreach (var year in available_financial_years)
                {
                    year.IsCurrent = false;
                    context.Entry(year).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
        }
    }

    [HttpPut]
    [Route("UpdateFinancialYear/{id}")]
    public async Task<IActionResult> UpdateFinancialYear([FromRoute] int id, [FromBody] FinancialYear financialYear)
    {
        try
        {
            if (financialYear.Id != id)
                return BadRequest();

            if (financialYear == null)
                return BadRequest("Financial year details can not be null");

            await CheckIfCurrentFinancialYearExists(financialYear.IsCurrent);

            context.Entry(financialYear).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostFinancialYear), new { id = financialYear.Id }, financialYear);

        }
        catch (System.Exception ex)
        {
            return BadRequest($"Error {ex}");
        }
    }
    [HttpDelete]
    [Route("RemoveFinancialYear/{id}")]
    public async Task<IActionResult> RemoveFinancialYear(int id)
    {
        try
        {
            var financialYear = await context.FinancialYear.FindAsync(id);
            if (financialYear == null)
            {
                return NotFound($"Financial year with ID {id} not found.");
            }

            context.FinancialYear.Remove(financialYear);
            await context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it based on your application's requirements.
            return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
        }
    }

    private bool FinancialYearExists(int year)
    {
        return (context.FinancialYear?.Any(i => i.Year == year)).GetValueOrDefault();
    }
}