using Microsoft.IdentityModel.Tokens;
using SpraywallAppWeb.Helpers;
using SpraywallAppWeb.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpraywallAppWeb.Services;
public class AuthService
{
    // Generate a security token, which may be used to authenticate
    // the user's requests to protected api endpoints.
    // To be called from sign in/up endpoints, on valid requests.
    // 
    // Hashing algorithm is HMAC SHA-256
    public string GenerateToken(User user)
    {
        JwtSecurityTokenHandler handler = new();

        // Retrieve private key from static helper class, use it to create signing credentials
        byte[] key = Encoding.ASCII.GetBytes(AuthSettings.PrivateKey);
        SigningCredentials credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            // Add data about the user to the token
            Subject = GenerateClaims(user),

            // Token expires after 1 day.
            // If mobile/desktop app gets 'token expired' error, they must sign in again
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials,
        };

        // Create and return the token
        SecurityToken token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    // Add data about the user to the token
    private static ClaimsIdentity GenerateClaims(User user)
    {
        ClaimsIdentity claims = new();

        // Add the user's ID to the token's claims
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        return claims;
    }
}
