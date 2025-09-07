using Application;
using CertiPoint.Middlewares;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecurityKey")))
        };
    });

if (Environment.GetEnvironmentVariable("AddAuthorization") == "Y")
    builder.Services.AddAuthorization(options => { options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); });

builder.Services.AddControllers(options =>
{
    if (Environment.GetEnvironmentVariable("AddExceptionLog") == "Y") options.Filters.Add<RequestResult>();
});

builder.Services.AddOpenApi();

builder.Services.Scan(scan => scan
    .FromAssemblies(//Assembly.Load("Infrastructure"), Assembly.Load("Application")
    Assembly.GetAssembly(typeof(DataBase)),
    Assembly.GetAssembly(typeof(SystemBusiness))
    )
    .AddClasses()
    .AsImplementedInterfaces()
    .WithScopedLifetime());

var app = builder.Build();

if (Environment.GetEnvironmentVariable("AddExceptionLog") == "Y") app.UseMiddleware<RequestLogger>();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
