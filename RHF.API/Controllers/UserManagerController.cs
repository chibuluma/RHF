using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RHF.Shared;


[Route("api/[controller]")]
[ApiController]
public class UserManagerController : ControllerBase {
    readonly RoleManager<IdentityRole> roleManager;
    readonly UserManager<IdentityUser> userManager;
    public UserManagerController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser>  userManager)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    [HttpGet]
    [Route("GetRoles")]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles(){
        if(roleManager.Roles == null)
            return NotFound();

        return await roleManager.Roles.ToListAsync();
    }

    [HttpGet]
    [Route("GetUsers")]
    public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers(){
        if(userManager.Users == null)
            return NotFound();

        return await userManager.Users.ToListAsync();
    }

    //POST /api/roles
    [HttpPost]
    [Route("AddUserRole/{roleName}")]
    public async Task<ActionResult> AddUserRole(string roleName)
    {
        if(roleName == null)
            return NotFound();

        try
        {
            await roleManager.CreateAsync(new IdentityRole() { Name = roleName });
            return Ok(); 
        }
        catch (Exception ex)
        {   
            return BadRequest(ex);
        }
    }

    //POST /api/roles
    [HttpPost]
    [Route("MapUserRole")]
    public async Task<ActionResult> MapUserRole([FromBody] UserRoleRequest model)
    {
        if(string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.RoleName))
            return NotFound();

        try
        {
            StringBuilder message = new StringBuilder();
            
            var user = await userManager.FindByNameAsync(model.UserName);

            var result = await userManager.AddToRoleAsync(user, model.RoleName);

            if(result.Succeeded)
            {    
                message.Append($"{model.UserName} mapped to Role:{model.RoleName} successfully."); 
                // Add additional claims if needed
                
                var role = await roleManager.Roles.FirstOrDefaultAsync(s=>s.Name == model.RoleName);
                
                if(role ==null){ // needs to be refactored
                    message.Append($"Role does not exist failed to mapped role claim to user");
                    return BadRequest(message);
                }
                var claim =new Claim(ClaimTypes.Role, model.RoleName);

                var addClaimsResult = await roleManager.AddClaimAsync(role, claim); // add role claim
                
                var addUserClaimResult = await userManager.AddClaimAsync(user, claim); // add user claim

                if (addClaimsResult.Succeeded && addUserClaimResult.Succeeded)
                {
                    message.Append($"Claims added successfully.");
                }
                else
                {
                    Console.WriteLine($"Error adding claims:");
                    message.Append($"Error adding claims: Role: {addClaimsResult.Errors} User: {addUserClaimResult.Errors}");
                }

                return Ok(message);
            }
            else
                return BadRequest(result.Errors);

        }
        catch (Exception ex)
        {   
            return BadRequest(ex);
        }
    }
    
    [HttpGet("validate-claim")]
    [Authorize] // Add necessary authorization attributes
    public IActionResult ValidateClaim([FromQuery] string claimType)
    {
        var hasClaim = User.HasClaim(c => c.Type == claimType);
        return Ok(new { HasClaim = hasClaim });
    }

    [HttpGet("manage/info")]
    [Authorize] // Add necessary authorization attributes
    public async Task<IActionResult> GetCustomUserInfo()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var claims = await userManager.GetClaimsAsync(user);
        
        var _claims = new List<CustomClaim>();

        foreach (var claim in claims)
        {
            _claims.Add(new CustomClaim(claim.Type, claim.Value));
        }

        var customInfo = new UserInfo
        {
            Email = user.Email,
            IsEmailConfirmed = user.EmailConfirmed,
            Claims = _claims
            // Add other custom claims or information as needed
        };

        return Ok(customInfo);
    }

}

