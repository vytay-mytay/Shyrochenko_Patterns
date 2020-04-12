using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Bridge.Implementation
{
    public class BridgeAdminImplementation : IBridgeImplementation
    {
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private UserManager<ApplicationUser> _userManager;

        public BridgeAdminImplementation(IUnitOfWork unitOfWork, IJWTService jwtService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            var request = model as AdminLoginRequestModel;
            var user = _unitOfWork.Repository<ApplicationUser>().Get(x => x.Email == request.Email)
                .TagWith(nameof(BridgeAdminImplementation) + "_GetAdmin")
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefault();

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password) || !user.UserRoles.Any(x => x.Role.Name == Role.Admin || x.Role.Name == Role.SuperAdmin))
                throw new CustomException(HttpStatusCode.BadRequest, "general", "Invalid credentials");

            return await _jwtService.BuildLoginResponse(user, request.AccessTokenLifetime);
        }

        public Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
