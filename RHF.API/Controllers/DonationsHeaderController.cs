using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
public class DonationsHeaderController : ControllerBase {
    private readonly RhfDbContext context;

    public DonationsHeaderController(RhfDbContext dbcontext)
    {
        context = dbcontext;
    }

    //GET /api/donations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonationsHeader>>> Getdonation(){
        try
        {
            if(context.DonationsHeaders == null)
            
            return NotFound();

        return await context.DonationsHeaders.ToListAsync(); 
        }
        catch (System.Exception)
        {
            
            throw;
        }

    }

        //GET /api/donations
    [HttpGet]
    [Route("GetDonationHeader")]
    public async Task<IEnumerable<DonationsHeaderDTO>> GetDonation(){
        try
        {
            var result = await PerformMapping(await context.DonationsHeaders.ToListAsync());
            
            return result; 

        }
        catch (System.Exception)
        {
            return new List<DonationsHeaderDTO>();
        }

    }

    private async Task<IList<DonationsHeaderDTO>> PerformMapping(IEnumerable<DonationsHeader> donations){
        var beneficiaryDto = new List<DonationsHeaderDTO>();

        foreach (var donation in donations)
        {
            beneficiaryDto.Add(new DonationsHeaderDTO(){
                Id = donation.Id,
                Period = donation.Period,
                Recipient = donation.Recipient,
                TotalAmountSpent = await CalculatTotal(donation.Id)
            });
        }
        return beneficiaryDto;
    }

    private async Task<double> CalculatTotal(int id)
    {
        var total = 0.0;

        var details = await context.DonationsDetails
            .Where(s=>s.DonationsHeaderId == id).ToListAsync();

        if(details != null)
           total =  details.Sum(s=>s.Total);
        
        return total;
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
