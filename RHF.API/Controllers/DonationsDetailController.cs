using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class DonationsDetailController : ControllerBase {
    private readonly RhfDbContext context;

    public DonationsDetailController(RhfDbContext dbcontext)
    {
        context = dbcontext;
    }

    //GET /api/donationDetailss
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonationsDetail>>> GetDonationsDetails(){
        if(context.DonationsDetails == null)
            return NotFound();

        return await context.DonationsDetails.ToListAsync();
    }

    //GET /api/donationDetailss/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DonationsDetail>> GetDonationsDetails(int id){
        if(context.DonationsDetails == null)
            return NotFound();

        var donationDetails = await context.DonationsDetails.FindAsync(id); 

        if(donationDetails == null)
            return NotFound();

        return donationDetails;
    }

    //POST /api/donationDetailss
    [HttpPost]
    public async Task<ActionResult<DonationsDetail>> PostdonationDetailsContribution(DonationsDetail donationDetails){
        context.Add(donationDetails);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDonationsDetails), new {id = donationDetails.Id}, donationDetails);
    }
    //PUT /api/donationDetailss
    [HttpPut("{id}")]
    public async Task<IActionResult> PutdonationDetailssContributions(int id, DonationsDetail donationDetails) {
        if(donationDetails.Id != id)
            return BadRequest();

        context.Entry(donationDetails).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
            if(!donationDetailsExists(id)){
                return NotFound();
            }else{
                throw;
            }
        }
         return NoContent();
    }

    //DELETE /api/donationDetails
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletedonationDetailsContribution(int id){
        if(context.DonationsDetails == null)
            return NotFound();

        var donationDetails = await context.DonationsDetails.FindAsync(id);
        if(donationDetails == null)
            return NotFound();

        context.DonationsDetails.Remove(donationDetails);
        await context.SaveChangesAsync();

        return NoContent();
    }
    private bool donationDetailsExists(int id){
        return (context.DonationsDetails?.Any(i=>i.Id == id)).GetValueOrDefault();
    }
}
