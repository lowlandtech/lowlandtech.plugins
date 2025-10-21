var builder = WebApplication.CreateBuilder(args);

// Load plugins configuration
// builder.Configuration.AddJsonFile("plugins.json", optional: false, reloadOnChange: true);
//{
//    "Plugins": [
//    {
//        "Name": "LowlandTech.Sample.Backend",
//        "IsActive": true
//    }
//  ]
//}

builder.Services.AddPlugin<BackendPlugin>();
builder.Services.AddPlugins();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UsePlugins();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

// Make Program accessible to integration tests
public partial class Program { }