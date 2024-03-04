using Microsoft.AspNetCore.Mvc;
using RHF.API.Services;

[ApiController]
[Route("api/[controller]")]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;
    //injecting the IMailService into the constructor
    public MailController(IMailService _MailService)
    {
        _mailService = _MailService;
    }

    [HttpPost]
    [Route("SendMail")]
    public ActionResult SendMail(MailData mailData)
    {
        try
        {
            var result =  _mailService.SendMail(mailData);
            if(result)
                return Ok();
            else{
                return BadRequest();
            }
        }
        catch (System.Exception)
        {
            throw;
        }

    }
}