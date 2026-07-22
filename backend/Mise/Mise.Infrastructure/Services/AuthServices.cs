using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mise.Application;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Services
{
    public class AuthServices : IAuthService
    {

        private readonly MiseDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthServices(MiseDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> GenerateTokenAsync(User user, string roleName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User?> ValidateUserAsync(string email, string password, Guid tenantId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                u.Email == email &&
                u.TenantId == tenantId &&
                u.Status == "active");

            if (user == null) return null;
            //if (user.PasswordHash != password) return null;
            if (!VerifyPassword(password, user.PasswordHash)) return null;

            return user;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var userRole = await _context.UserRoles.Include(ur => ur.Role).FirstOrDefaultAsync(u => u.UserId == userId);

            return userRole?.Role?.Name ?? "cook";
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, Guid tenantId)
        {
            //revoke any existing tokens
            var exisitingTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach(var existing in exisitingTokens)
            {
                existing.IsRevoked = true;
                existing.RevokedAt = DateTime.UtcNow;
            }

            if (exisitingTokens.Any()) await _context.SaveChangesAsync();

            // generate a new secure token
            var tokenBytes = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            var tokenString = Convert.ToBase64String(tokenBytes);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                TenantId = tenantId,
                Token = tokenString,
                ExpiresAt = DateTime.UtcNow.AddHours(12),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<(User? user, string roleName)> ValidateRefreshTokenAsync(string token, Guid tenantId)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    rt.TenantId == tenantId &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);

            if (refreshToken == null)
                return (null, string.Empty);

            var roleName = await GetUserRoleAsync(refreshToken.UserId);
            return (refreshToken.User, roleName);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null) return;

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                    .ThenInclude(rp => rp.RolePermissions)
                        .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            return userRole.Role.RolePermissions
                .Select(rp => $"{rp.Permission.Resource}.{rp.Permission.Action}")
                .ToList();

        }
    }
}
