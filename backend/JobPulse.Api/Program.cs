using JobPulse.Core.Data;
using JobPulse.Core.Scrapers;
using JobPulse.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

// ============================================================================
// JWT AUTHENTICATION SETUP
// ============================================================================
// This configures HOW tokens will be validated. The actual validation happens
// in the middleware (UseAuthentication) on each request.
//
// Flow:
// 1. AddAuthentication() - registers authentication services in DI container
// 2. AddJwtBearer() - configures JWT Bearer as the authentication scheme
// 3. Authority URL tells the middleware where to fetch public keys (JWKS)
// ============================================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Authority = Supabase Auth URL
        // On first request, middleware fetches:
        //   {Authority}/.well-known/openid-configuration -> contains jwks_uri
        //   {Authority}/.well-known/jwks.json -> contains public keys for ES256
        // Keys are cached and refreshed automatically
        options.Authority = "https://whlylisnxodwcrrevvvt.supabase.co/auth/v1";

        // Audience = expected "aud" claim in token
        // Supabase sets aud="authenticated" for logged-in users
        options.Audience = "authenticated";

        // What to validate in each token:
        options.TokenValidationParameters.ValidateIssuer = true;    // iss must match Authority
        options.TokenValidationParameters.ValidateAudience = true;  // aud must match Audience
        options.TokenValidationParameters.ValidateLifetime = true;  // exp must be in future
    });

builder.Services.AddEndpointsApiExplorer();  // Required for Swagger
builder.Services.AddSwaggerGen();             // Generates API documentation

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();      // Serves swagger.json
app.UseSwaggerUI();    // Serves Swagger UI at /swagger

app.UseHttpsRedirection();

// ============================================================================
// AUTHENTICATION & AUTHORIZATION MIDDLEWARE
// ============================================================================
// UseAuthentication() - runs on EVERY request:
//   1. Looks for "Authorization: Bearer <token>" header
//   2. If found, parses the JWT token
//   3. Fetches public key from JWKS (cached)
//   4. Validates signature (ES256), issuer, audience, expiration
//   5. If valid: extracts claims and sets HttpContext.User
//   6. If invalid: HttpContext.User remains anonymous (no error yet!)
//
// UseAuthorization() - runs after authentication:
//   1. Checks if endpoint has [Authorize] attribute
//   2. If yes and User is anonymous -> returns 401 Unauthorized
//   3. If yes and User is authenticated -> allows request through
//   4. If no [Authorize] -> allows request through (public endpoint)
// ============================================================================
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
