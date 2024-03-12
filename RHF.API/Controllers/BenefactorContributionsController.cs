using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;
using SQLitePCL;

[Route("api/[controller]")]
[ApiController]
public class BenefactorContributionsController : ControllerBase
{
    private readonly RhfDbContext context;
    private readonly IMapper mapper;

    public BenefactorContributionsController(RhfDbContext dbcontext, IMapper mapper)
    {
        context = dbcontext;
        this.mapper = mapper;
    }

    //GET /api/Benefactors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BenefactorContribution>>> GetBenefactorsContributions()
    {
        if (context.BenefactorContributions == null)
            return NotFound();

        return await context.BenefactorContributions.ToListAsync();
    }

    //GET /api/Benefactors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<IList<BenefactorContribution>>> GetBenefactorContributions(int id)
    {
        if (context.BenefactorContributions == null)
            return NotFound();

        var benefactorContributions = await context.BenefactorContributions
            .Where(i => i.BenefactorId == id)
            .ToListAsync();

        if (benefactorContributions == null)
            return NotFound();

        return benefactorContributions;
    }

    //POST /api/Benefactors
    [HttpPost]
    public async Task<ActionResult<BenefactorContribution>> PostBenefactorContribution(BenefactorContributionsDTO benefactorContributionDTO)
    {
        // Validate the model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Map DTO to BenefactorContribution entity
        var benefactorContributionInfo = mapper.Map<BenefactorContribution>(benefactorContributionDTO);

        try
        {
            // Add benefactor contribution to the context
            benefactorContributionInfo.FinancialYearId = GetCurrentFinancialYear()
            .GetValueOrDefault(); // sets the current financial year to contribution
            context.Add(benefactorContributionInfo);

            // Save changes to the database
            await context.SaveChangesAsync();

            // Return the created benefactor contribution with a 201 Created status
            return CreatedAtAction(nameof(GetBenefactorContributions), new { id = benefactorContributionInfo.Id }, benefactorContributionInfo);
        }
        catch (Exception ex)
        {
            // Handle database-related errors
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    private int? GetCurrentFinancialYear()
    {
        var year = context.FinancialYear.Where(s=>s.IsCurrent)
            .FirstOrDefault()?.Id;

        return year;
    }

    //PUT /api/Benefactors
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBenefactorsContributions(int id, BenefactorContribution benefactor)
    {
        if (benefactor.Id != id)
            return BadRequest();

        context.Entry(benefactor).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
            if (!BenefactorExists(id))
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

    //DELETE /api/Benefactor
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBenefactorContribution(int id)
    {
        var benefactor = await context.BenefactorContributions.FindAsync(id);
        
        if (benefactor == null)
            return NotFound();

        context.BenefactorContributions.Remove(benefactor);
        await context.SaveChangesAsync();

        return NoContent();
    }
    private bool BenefactorExists(int id)
    {
        return (context.BenefactorContributions?.Any(i => i.Id == id)).GetValueOrDefault();
    }
}
