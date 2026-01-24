using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPulse.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Returns current authenticated user info.
    /// Token must be passed in Authorization header: "Bearer {token}"
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult GetCurrentUser()
    {
        // All standard ClaimTypes and their values from the token
        return Ok(new
        {
            // Standard .NET ClaimTypes
            Claims = new
            {
                NameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,  // User ID
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                Name = User.FindFirst(ClaimTypes.Name)?.Value,
                GivenName = User.FindFirst(ClaimTypes.GivenName)?.Value,
                Surname = User.FindFirst(ClaimTypes.Surname)?.Value,
                MobilePhone = User.FindFirst(ClaimTypes.MobilePhone)?.Value,
                AuthenticationMethod = User.FindFirst(ClaimTypes.AuthenticationMethod)?.Value,
                Expiration = User.FindFirst(ClaimTypes.Expiration)?.Value
            },

            // Raw claims that don't have ClaimTypes constant (Supabase-specific)
            RawClaims = new
            {
                Issuer = User.FindFirst("iss")?.Value,
                Audience = User.FindFirst("aud")?.Value,
                IssuedAt = User.FindFirst("iat")?.Value,
                Expiration = User.FindFirst("exp")?.Value,
                SessionId = User.FindFirst("session_id")?.Value,
                AuthLevel = User.FindFirst("aal")?.Value,
                IsAnonymous = User.FindFirst("is_anonymous")?.Value
            }
        });
    }
}
