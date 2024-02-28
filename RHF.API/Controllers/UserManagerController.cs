using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using RHF.DAL;
using RHF.Shared;


[Route("api/[controller]")]
[ApiController]
public class UserManagerController : ControllerBase
{
    readonly RoleManager<IdentityRole> roleManager;
    readonly UserManager<IdentityUser> userManager;
    readonly RhfDbContext context;
    public UserManagerController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, RhfDbContext context)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.context = context;
    }

    [HttpGet]
    [Route("GetRoles")]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
    {
        if (roleManager.Roles == null)
            return NotFound();

        return await roleManager.Roles.ToListAsync();
    }

    [HttpGet]
    [Route("GetUsers")]
    public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
    {
        if (userManager.Users == null)
            return NotFound();

        return await userManager.Users
        .ToListAsync();
    }

    //POST /api/roles
    [HttpPost]
    [Route("AddUserRole/{roleName}")]
    public async Task<ActionResult> AddUserRole(string roleName)
    {
        if (roleName == null)
            return NotFound();

        try
        {
            var response = await roleManager.CreateAsync(new IdentityRole() { Name = roleName });

            if (response.Succeeded)
                return Ok();

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    [HttpGet]
    [Route("GetUsersAndRoles")]
    public async Task<ActionResult<UserRolesResponse>> GetUserAndRoles()
    {
        UserRolesResponse users_collection = new();

        var users = await userManager.Users.ToListAsync(); ;

        if (users != null)
        {
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                users_collection.UserRoles.Add(user.UserName, roles);
            }
        }

        return Ok(users_collection);
    }

    //POST /api/roles
    [HttpPost]
    [Route("MapUserRole")]
    public async Task<ActionResult> MapUserRole([FromBody] UserRoleRequest model)
    {
        if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.RoleName))
            return NotFound();

        try
        {
            StringBuilder message = new StringBuilder();

            var user = await userManager.FindByNameAsync(model.UserName);

            var result = await userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                message.Append($"{model.UserName} mapped to Role:{model.RoleName} successfully.");
                // Add additional claims if needed

                var role = await roleManager.Roles.FirstOrDefaultAsync(s => s.Name == model.RoleName);

                if (role == null)
                { // needs to be refactored
                    message.Append($"Role does not exist failed to mapped role claim to user");
                    return BadRequest(message);
                }
                var claim = new Claim(ClaimTypes.Role, model.RoleName);

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
    [HttpDelete]
    [Route("RemoveUserRole")]
    public async Task<ActionResult> RemoveUserRole([FromBody] UserRoleRequest model)
    {
        if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.RoleName))
            return NotFound();

        try
        {
            StringBuilder message = new StringBuilder();

            var user = await userManager.FindByNameAsync(model.UserName);

            var result = await userManager.RemoveFromRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                message.Append($"{model.UserName} removed from Role:{model.RoleName} successfully.");
                // Add additional claims if needed

                var role = await roleManager.Roles.FirstOrDefaultAsync(s => s.Name == model.RoleName);

                if (role == null)
                { // needs to be refactored
                    message.Append($"Role does not exist failed to mapped role claim to user");
                    return BadRequest(message);
                }
                var claim = new Claim(ClaimTypes.Role, model.RoleName);

                var addClaimsResult = await roleManager.RemoveClaimAsync(role, claim); // add role claim

                var addUserClaimResult = await userManager.RemoveClaimAsync(user, claim); // add user claim

                if (addClaimsResult.Succeeded && addUserClaimResult.Succeeded)
                {
                    message.Append($"Claims removed successfully.");
                }
                else
                {
                    Console.WriteLine($"Error removing claims:");
                    message.Append($"Error removing claims: Role: {addClaimsResult.Errors} User: {addUserClaimResult.Errors}");
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

        //check if user is mapped

        var customInfo = new UserInfo
        {
            Email = user.Email,
            IsEmailConfirmed = user.EmailConfirmed,
            Claims = _claims
            // Add other custom claims or information as needed
        };

        return Ok(customInfo);
    }
    [HttpGet("Users/Info")]
    public async Task<IActionResult> GeUserInfo()
    {
        var users = await userManager.Users.ToListAsync();

        if (users == null)
        {
            return NotFound();
        }

        var userInfo = new List<UserInfo>();

        foreach (var user in users)
        {
            userInfo.Add(
                new UserInfo
                {
                    Email = user.Email,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsUserAccountMapped = IsUserAccountMappedToBenefactor(user.Email),
                }
            );
        }

        return Ok(userInfo);
    }


    private bool IsUserAccountMappedToBenefactor(string user)
    {
        var result = context.Benefactors.Any(s => s.UserId == user);
        return result;
    }

    [HttpPost("UpdatePassword")]
    public async Task<IActionResult> UpdatePassword(UserPassword userPassword)
    {
        try
        {
            if (userPassword == null || string.IsNullOrWhiteSpace(userPassword.UserName) ||
                string.IsNullOrWhiteSpace(userPassword.Token) || string.IsNullOrWhiteSpace(userPassword.Password))
            {
                return BadRequest("User, password, or token cannot be null or empty");
            }

            var identityUser = await userManager.FindByEmailAsync(userPassword.UserName);

            if (identityUser == null)
            {
                return BadRequest("User not found");
            }

            var result = await userManager.ResetPasswordAsync(identityUser, userPassword.Token, userPassword.Password);

            if (result.Succeeded)
            {
                return Ok("Password updated successfully");
            }
            else
            {
                // Log errors for troubleshooting

                return BadRequest($"Failed to update password: {result.Errors}");
            }
        }
        catch (Exception ex)
        {
            // Log the exception for troubleshooting
            // Example: logger.LogError($"Exception updating password: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }
    [HttpPost("User/ConfirmUserAccount/{user}")]
    public async Task<IActionResult> ConfirmUserAccount(string user)
    {
        try
        {
            if (user == null)
            {
                return BadRequest("User, password, or token cannot be null or empty");
            }

            var identityUser = await userManager.FindByEmailAsync(user);

            if (identityUser == null)
            {
                return BadRequest("User not found");
            }

            identityUser.EmailConfirmed = true;
            var result = await userManager.UpdateAsync(identityUser);

            if (result.Succeeded)
            {
                return Ok("User confirmed successfully");
            }
            else
            {
                // Log errors for troubleshooting

                return BadRequest($"Failed to confirm user: {result.Errors}");
            }
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

    }
    [HttpGet("User/IsUserAccountConfirmed/{id}")]
    public async Task<IActionResult> IsUserAccountConfirmed(string id)
    {
        try
        {
            if (id == null)
            {
                return BadRequest("User, password, or token cannot be null or empty");
            }

            var identityUser = await userManager.FindByEmailAsync(id);

            if (identityUser == null)
            {
                return BadRequest("User not found");
            }

            if(identityUser.EmailConfirmed)
                return Ok();
            else
                return BadRequest();

        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

    }
    [HttpPost("User/UnConfirmUserAccount/{user}")]
    public async Task<IActionResult> UnConfirmUserAccount(string user)
    {
        try
        {
            if (user == null)
            {
                return BadRequest("User, password, or token cannot be null or empty");
            }

            var identityUser = await userManager.FindByEmailAsync(user);

            if (identityUser == null)
            {
                return BadRequest("User not found");
            }

            identityUser.EmailConfirmed = false;
            var result = await userManager.UpdateAsync(identityUser);

            if (result.Succeeded)
            {
                return Ok("User unconfirmed successfully");
            }
            else
            {
                // Log errors for troubleshooting

                return BadRequest($"Failed to confirm user: {result.Errors}");
            }
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

    }
}

