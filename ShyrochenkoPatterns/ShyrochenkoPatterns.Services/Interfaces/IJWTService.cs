using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.ResponseModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces
{
    public interface IJWTService
    {
        Task<ClaimsIdentity> GetIdentity(ApplicationUser user, bool isRefreshToken);

        JwtSecurityToken CreateToken(DateTime now, ClaimsIdentity identity, DateTime lifetime);

        Task<TokenResponseModel> CreateUserTokenAsync(ApplicationUser user, int? lifetime = null, bool isRefresh = false);

        Task<LoginResponseModel> BuildLoginResponse(ApplicationUser user, int? accessTokenLifetime = null);

        Task ClearUserTokens(ApplicationUser user);
    }
}
