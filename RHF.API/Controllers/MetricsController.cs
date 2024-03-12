using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.DAL;

[Route("api/[controller]")]
[ApiController]
public class MetricsController : ControllerBase
{
    readonly RhfDbContext dbContext;
    public MetricsController(RhfDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    [HttpGet]
    [Route("GetAggregatedContributionsByYearAndMonth")]
    public IActionResult GetAggregatedContributionsByYearAndMonth(){
        try
        {
            var result = dbContext.BenefactorContributions
            .GroupBy(t => new { Month = t.DatePaid.Month, Year = t.DatePaid.Year })
            .Select(group => new
            {
                Month = group.Key.Month,
                Year = group.Key.Year,
                TotalAmount = group.Sum(t => t.AmountPaid)
            })
             .OrderBy(group => group.Year)
            .ThenBy(group => group.Month)
            .ToList();

            return Ok(result);
        }
        catch (System.Exception e)
        {     
            return BadRequest($"Error:  {e}");
        }

    }
    [HttpGet]
    [Route("GetMetrics/{userId}")]
    public async Task<IActionResult> GetMetrics([FromRoute] string userId)
    {
        try
        {
            var metric = new Metrics();

            var id = GetBenefactorIdFromUserName(userId);

            if (id != 0)
            {
                metric.TotalBenefactors = await dbContext.Benefactors.CountAsync();

                metric.TotalContributions = await dbContext.BenefactorContributions
                    .SumAsync(s => s.AmountPaid);

                metric.MyTotalContributions = await dbContext.BenefactorContributions
                    .Where(u => u.BenefactorId == id)
                    .SumAsync(s => s.AmountPaid);

                metric.ContributedVsRaised[0] = await dbContext.BenefactorContributions
                    .SumAsync(s => s.AmountPaid);

                metric.ContributedVsRaised[1] = await dbContext.DonationsDetails
                    .SumAsync(s => s.Total);

                metric.NextProjectSchedule = await dbContext.ProjectTasks
                    .OrderBy(d => d.DateStart)
                    .Where(s => s.IsDone != true)
                    .Select(d => d.DateStart)
                    .FirstOrDefaultAsync();
            }

            return Ok(metric);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex);
        }
    }


    private int GetBenefactorIdFromUserName(string userId)
    {

        var benefactor = dbContext.Benefactors.Where(i => i.UserId == userId)
        .FirstOrDefault();

        if (benefactor != null)
            return benefactor.Id;
        else
            return 0;

    }
}
