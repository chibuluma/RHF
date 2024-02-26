using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "CFO")]
public class DonationsHeaderController : ControllerBase {
    private readonly RhfDbContext context;

    public DonationsHeaderController(RhfDbContext dbcontext)
    {
        context = dbcontext;
    }

    //GET /api/donations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonationsHeader>>> Getdonation(){
        if(context.DonationsHeaders == null)
            return NotFound();

        return await context.DonationsHeaders.ToListAsync();
    }

    //GET /api/donations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DonationsHeader>> Getdonation(int id){
        if(context.DonationsHeaders == null)
            return NotFound();

        var donation = await context.DonationsHeaders.FindAsync(id); 

        if(donation == null)
            return NotFound();

        return donation;
    }

    //POST /api/donations
    [HttpPost]
    public async Task<ActionResult<DonationsHeader>> PostDonation(DonationsHeader donation){
        context.Add(donation);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Getdonation), new {id = donation.Id}, donation);
    }
    //PUT /api/donations
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDonationHeaderDetailsContributions(int id, DonationsHeader donation) {
        if(donation.Id != id)
            return BadRequest();

        context.Entry(donation).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (System.Exception)
        {
            if(!donationExists(id)){
                return NotFound();
            }else{
                throw;
            }
        }
         return NoContent();
    }

    //DELETE /api/donation
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletedonationContribution(int id){
        if(context.DonationsHeaders == null)
            return NotFound();

        var donation = await context.DonationsHeaders.FindAsync(id);
        if(donation == null)
            return NotFound();

        context.DonationsHeaders.Remove(donation);
        await context.SaveChangesAsync();

        return NoContent();
    }
    private bool donationExists(int id){
        return (context.DonationsHeaders?.Any(i=>i.Id == id)).GetValueOrDefault();
    }
}
