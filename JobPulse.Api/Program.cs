using JobPulse.Core.Scrapers;
using JobPulse.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Register HttpClient for scrapers
// AddHttpClient creates HttpClient factory with proper lifecycle
builder.Services.AddHttpClient<IJobScraper, LinkedInScraper>();

// Register JobService
// AddScoped = new instance for each HTTP request
builder.Services.AddScoped<JobService>();

// Register JobService
// AddScoped = a new instance for each HTTP request
builder.Services.AddScoped<JobService>();

builder.Services.AddEndpointsApiExplorer();  // Needed for Swagger
builder.Services.AddSwaggerGen();             // Generate documentation

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();      // Includes /swagger/v1/swagger.json
    app.UseSwaggerUI();    // Enables the UI at /swagger
//}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
