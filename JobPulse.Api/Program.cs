using JobPulse.Core.Data;
using JobPulse.Core.Scrapers;
using JobPulse.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with PostgreSQL (Supabase)
// Reads connection string from appsettings.json -> ConnectionStrings:DefaultConnection
builder.Services.AddDbContext<JobPulseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register controllers
builder.Services.AddControllers();

// Register HttpClient for scrapers
// AddHttpClient creates HttpClient factory with proper lifecycle
builder.Services.AddHttpClient<IJobScraper, LinkedInScraper>();

// Register JobService
// AddScoped = new instance for each HTTP request
builder.Services.AddScoped<JobService>();

builder.Services.AddEndpointsApiExplorer();  // Needed for Swagger
builder.Services.AddSwaggerGen();             // Generate documentation

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();      // Includes /swagger/v1/swagger.json
app.UseSwaggerUI();    // Enables the UI at /swagger

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
