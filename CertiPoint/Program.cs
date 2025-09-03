using Application;
using Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
