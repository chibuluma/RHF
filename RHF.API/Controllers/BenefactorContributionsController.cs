using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;
using SQLitePCL;

[Route("api/[controller]")]
[ApiController]
public class BenefactorContributionsController : ControllerBase {
    private readonly RhfDbContext context;
    private readonly IMapper mapper;

    public BenefactorContributionsController(RhfDbContext dbcontext, IMapper mapper)
    {
        context = dbcontext;
        this.mapper = mapper;
    }

    //GET /api/Benefactors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BenefactorContribution>>> GetBenefactorsContributions(){
        if(context.BenefactorContributions == null)
            return NotFound();

        return await context.BenefactorContributions.ToListAsync();
    }

    //GET /api/Benefactors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<IList<BenefactorContribution>>> GetBenefactorContributions(int id){
        if(context.BenefactorContributions == null)
            return NotFound();

        var benefactorContributions = await context.BenefactorContributions
            .Where(i=>i.BenefactorId == id)
            .ToListAsync();

        if(benefactorContributions == null)
            return NotFound();

        return benefactorContributions;
    }

    //POST /api/Benefactors
    [HttpPost]
    [Authorize(Roles = "CFO")]
    public async Task<ActionResult<Benefactor>> PostBenefactorContribution(BenefactorContributionsDTO benefactorContributionDTO){
        var benefactorContributionInfo = mapper.Map<BenefactorContribution>(benefactorContributionDTO);
        try
        {
            context.Add(benefactorContributionInfo);
            await context.SaveChangesAsync(); 
            return CreatedAtAction(nameof(GetBenefactorContributions), new {id = benefactorContributionInfo.Id}, benefactorContributionInfo); 
        }
        catch (System.Exception ex)
        { 
            throw;
        }
    }
    //PUT /api/Benefactors
    [HttpPut("{id}")]
    [Authorize(Roles = "CFO")]
    public async Task<IActionResult> PutBenefactorsContributions(int id, BenefactorContribution benefactor) {
        if(benefactor.Id != id)
            return BadRequest();

        context.Entry(benefactor).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
            if(!BenefactorExists(id)){
                return NotFound();
            }else{
                throw;
            }
        }
         return NoContent();
    }

    //DELETE /api/Benefactor
    [HttpDelete("{id}")]
    [Authorize(Roles = "CFO")]
    public async Task<IActionResult> DeleteBenefactorContribution(int id){
        if(context.BenefactorContributions == null)
            return NotFound();

        var benefactor = await context.BenefactorContributions.FindAsync(id);
        if(benefactor == null)
            return NotFound();

        context.BenefactorContributions.Remove(benefactor);
        await context.SaveChangesAsync();

        return NoContent();
    }
    private bool BenefactorExists(int id){
        return (context.BenefactorContributions?.Any(i=>i.Id == id)).GetValueOrDefault();
    }
}
