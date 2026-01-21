var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
