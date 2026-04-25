using DBS_Task.Application.Common.Interfaces;
using DBS_Task.Application.Common.Results;
using DBS_Task.Domain.Entities;
using DBS_Task.Infrastructure.Data.DBContext;
using DBS_Task.Infrastructure.Security;
using DBS_Task.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Hotel_Booking_API.Infrastructure.Services
{
    public class JwtService : IJWTService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
                private readonly ApplicationDbContext _dbContext;

        private readonly RefreshTokenSettings _refreshTokenSettings;

        public JwtService(
            IOptions<JwtSettings> jwtOptions,
            IOptions<RefreshTokenSettings> refreshTokenOptions,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _jwtSettings = jwtOptions.Value;
            _refreshTokenSettings = refreshTokenOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<TokenResult> GenerateTokenAsync(ApplicationUser user)
        {
            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var expirationMinutes = _jwtSettings.ExpirationMinutes;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti,
                  Guid.NewGuid().ToString())
            };

            // Fetch User Roles and add them as Claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));

                // Fetch the Role object to get its associated permissions/claims (AspNetRoleClaims)
                var identityRole = await _roleManager.FindByNameAsync(roleName);
                if (identityRole != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                    claims.AddRange(roleClaims);
                }
            }

            // Remove duplicate claims 
            claims = claims
                .DistinctBy(c => new { c.Type, c.Value }) 
                .ToList();

            var accessExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

            // Create Token Object
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: accessExpiry,
                signingCredentials: credentials
            );

            var refreshTokenValue =
               GenerateRefreshToken();

            var refreshExpiry =
               DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = refreshExpiry,
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            return new TokenResult
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiresAt = accessExpiry,
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiresAt = refreshExpiry,
                Roles = roles.ToList(),
                Claims = claims.Where(c => c.Type == "permission")
                               .Select(c => c.Value).ToList()
            };
        }


        private string GenerateRefreshToken()
        {
            var bytes =
              RandomNumberGenerator.GetBytes(_refreshTokenSettings.TokenSizeInBytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
