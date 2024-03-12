using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using RHF.Shared;

[Route("api/[controller]")]
[ApiController]
public class DonationsDetailController : ControllerBase {
    private readonly RhfDbContext context;
    private readonly IMapper mapper;

    public DonationsDetailController(RhfDbContext dbcontext,IMapper _mapper)
    {
        context = dbcontext;
        mapper = _mapper;
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

    [HttpGet]
    [Route("GetDonationsDetailsByHeaderId/{id}")]
    public async Task<ActionResult> GetDonationsDetailsByHeaderId(int id){
        if(context.DonationsDetails == null)
            return NotFound();

        var donationDetails = await context.DonationsDetails.Where(i=>i.DonationsHeaderId==id).ToListAsync();

        if(donationDetails == null)
            return NotFound();

        return Ok(donationDetails);
    }

    //POST /api/donationDetailss
    [HttpPost]
    [Route("PostDonationDetailsContribution")]
    public async Task<ActionResult> PostdonationDetailsContribution([FromBody] DonationsDetailDTO donationDetails){
        if(donationDetails == null)
            return BadRequest("Details object is null");
            try
            {
                // Map DTO to BenefactorDetails entity
                var donations = mapper.Map<DonationsDetail>(donationDetails);

                context.DonationsDetails.Add(donations);
                await context.SaveChangesAsync();
                return Ok();   
            }
            catch (System.Exception ex)
            {    
                return BadRequest(ex);
            }

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
