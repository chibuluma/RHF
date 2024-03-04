using Microsoft.EntityFrameworkCore;
using RHF.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RHF.API.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// cookie authentication
builder.Services
.AddAuthentication(IdentityConstants.ApplicationScheme)
.AddCookie("Identity.Bearer")
.AddIdentityCookies();

// configure authorization
builder.Services.AddAuthorizationBuilder();
builder.Services.AddControllers()
.AddNewtonsoftJson();

// Add services to the container.
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddDbContext<RhfDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("RHFContext")));
#pragma warning restore CS8604 // Possible null reference argument.


// builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddScoped<IMailService, MailService>();

// add identity and opt-in to endpoints
builder.Services.AddIdentityCore<IdentityUser>()
     .AddRoles<IdentityRole>()
     .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
     .AddEntityFrameworkStores<RhfDbContext>()
     .AddApiEndpoints();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RhfDbContext>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>  
{  
    options.AddPolicy("AllowSpecificOrigin", 
        policy  =>  
        {  
            policy.WithOrigins(builder.Configuration["AllowedClients"] ?? "https://localhost:7184")
            .AllowAnyHeader()
            .AllowAnyMethod()// add the allowed origins  
            .SetIsOriginAllowed(pol => true)
            .AllowCredentials();
        });  
});  
  
builder.Services.AddOptions<MailSettings>()
    .Bind(builder.Configuration.GetSection("MailSettings"), opt=>opt.ErrorOnUnknownConfiguration=true)
    .ValidateDataAnnotations();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// provide an end point to clear the cookie for logout
// The request checks for an empty body to prevent CSRF attacks. By requiring something
// in the body, the request must be made from JavaScript, which is the only way to
// access the cookie. It can't be accessed by a form-based post.
// This prevents a malicious site from logging the user out.
// Furthermore, the endpoint is protected by authorization to prevent anonymous access.
// The client simply needs to pass an empty object {} in the body of the request.
app.MapPost("/Logout", async (SignInManager<IdentityUser> signInManager, [FromBody] object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
})
.RequireAuthorization();

// Inside Configure method
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
// create routes for the identity endpoints
app.MapIdentityApi<IdentityUser>();

app.Run();


class AppUser : IdentityUser { }

