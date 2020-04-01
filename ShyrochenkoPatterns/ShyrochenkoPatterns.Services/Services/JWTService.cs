using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Common.Utilities;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services
{
    public class JWTService : IJWTService
    {
        private UserManager<ApplicationUser> _userManager = null;
        private HashUtility _hashService = null;
        private IUnitOfWork _unitOfWork;

        public JWTService(UserManager<ApplicationUser> userManager, HashUtility hashService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _hashService = hashService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClaimsIdentity> GetIdentity(ApplicationUser user, bool isRefreshToken)
        {
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("isRefresh", isRefreshToken.ToString())
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                }

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            return null;
        }

        public JwtSecurityToken CreateToken(DateTime now, ClaimsIdentity identity, DateTime lifetime)
        {
            return new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: lifetime,
                //signingCredentials: AuthOptions.GetSigningCredentials()
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
        }

        public async Task<TokenResponseModel> CreateUserTokenAsync(ApplicationUser user, int? accessTokenLifetime = null, bool isRefresh = false)
        {
            DateTime dateNow = DateTime.UtcNow;

            #region remove old tokens

            var tokens = _unitOfWork.Repository<UserToken>().Get(x => x.UserId == user.Id)
                .TagWith(nameof(CreateUserTokenAsync) + "_GetUsersTokens")
                .ToList();

            tokens.ForEach(x => _unitOfWork.Repository<UserToken>().DeleteById(x.Id));

            #endregion

            #region create token

            var accessIdentity = await GetIdentity(user, false);
            var refreshIdentity = await GetIdentity(user, true);

            if (accessIdentity == null || refreshIdentity == null)
            {
                throw new Exception("User not found");
            }

            var accessLifetime = accessTokenLifetime.HasValue && accessTokenLifetime.Value != 0 ? dateNow.Add(TimeSpan.FromSeconds(accessTokenLifetime.Value)) : dateNow.Add(TimeSpan.FromDays(AuthOptions.ACCESS_TOKEN_LIFETIME));
            var refreshLifetime = dateNow.Add(TimeSpan.FromDays(AuthOptions.REFRESH_TOKEN_LIFETIME));

            var accessJwtToken = new JwtSecurityTokenHandler().WriteToken(CreateToken(dateNow, accessIdentity, accessLifetime));
            var refreshJwtToken = new JwtSecurityTokenHandler().WriteToken(CreateToken(dateNow, refreshIdentity, refreshLifetime));

            user.Tokens.Add(new UserToken
            {
                AccessExpiresDate = accessLifetime,
                RefreshExpiresDate = refreshLifetime,
                IsActive = true,
                AccessTokenHash = _hashService.GetHash(accessJwtToken),
                RefreshTokenHash = _hashService.GetHash(refreshJwtToken),
                CreatedAt = DateTime.UtcNow
            });

            #endregion

            TokenResponseModel response = new TokenResponseModel
            {
                AccessToken = accessJwtToken,
                ExpireDate = accessLifetime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                RefreshToken = refreshJwtToken,
                Type = "Bearer"
            };

            _unitOfWork.Repository<ApplicationUser>().Update(user);
            _unitOfWork.SaveChanges();

            return response;
        }

        public async Task<LoginResponseModel> BuildLoginResponse(ApplicationUser user, int? accessTokenLifetime = null)
        {
            user.LastVisitAt = DateTime.UtcNow;

            _unitOfWork.Repository<ApplicationUser>().Update(user);
            _unitOfWork.SaveChanges();

            var tokenResponseModel = await CreateUserTokenAsync(user, accessTokenLifetime);

            var roles = await _userManager.GetRolesAsync(user);
            return new LoginResponseModel
            {
                Id = user.Id,
                Email = user.Email ?? null,
                PhoneNumber = user.PhoneNumber ?? null,
                Token = tokenResponseModel,
                Role = (roles != null) ? roles.SingleOrDefault() : "none"
            };
        }

        public async Task ClearUserTokens(ApplicationUser user)
        {
            var tokens = user.Tokens.ToList();

            tokens.ForEach(x => _unitOfWork.Repository<UserToken>().DeleteById(x.Id));

            _unitOfWork.SaveChanges();
        }
    }
}
