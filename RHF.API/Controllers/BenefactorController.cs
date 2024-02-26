using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
public class BenefactorController : ControllerBase {
    private readonly RhfDbContext context;
    private readonly IMapper mapper;
    public BenefactorController(RhfDbContext dbcontext, IMapper _mapper)
    {
        context = dbcontext;
        mapper = _mapper;
    }

    //GET /api/Benefactors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Benefactor>>> GetBenefactors(){
        if(context.Benefactors == null)
            return NotFound();

        return await context.Benefactors.ToListAsync();
    }

    //GET /api/Benefactors/5
    [HttpGet]
    [Route("GetBenefactorByUserId/{id}")]
    public async Task<ActionResult<Benefactor>> GetBenefactorByEmail(string id){
        if(context.Benefactors == null)
            return NotFound();

        var benefactor = await context.Benefactors
            .FirstOrDefaultAsync(i=>i.UserId == id); 

        if(benefactor == null)
            return NoContent();

        return benefactor;
    }
    //GET /api/Benefactors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Benefactor>> GetBenefactors(int id){
        if(context.Benefactors == null)
            return NotFound();

        var benefactor = await context.Benefactors.FindAsync(id); 

        if(benefactor == null)
            return NotFound();

        return benefactor;
    }

    //POST /api/Benefactors
    [HttpPost]
    public async Task<ActionResult<Benefactor>> PostBenefactors(BenefactorDTO benefactorDTO){
        var benefactorInfo = mapper.Map<Benefactor>(benefactorDTO);

        context.Add(benefactorInfo);
        await context.SaveChangesAsync();
 
        return CreatedAtAction(nameof(GetBenefactors), new {id = benefactorInfo.Id}, benefactorInfo);
    }
    //PUT /api/Benefactors
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBenefactors(int id, Benefactor benefactor) {
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
    public async Task<IActionResult> DeleteBenefactor(int id){
        if(context.Benefactors == null)
            return NotFound();

        var benefactor = await context.Benefactors.FindAsync(id);
        if(benefactor == null)
            return NotFound();

        context.Benefactors.Remove(benefactor);
        await context.SaveChangesAsync();

        return NoContent();
    }
    private bool BenefactorExists(int id){
        return (context.Benefactors?.Any(i=>i.Id == id)).GetValueOrDefault();
    }
}
