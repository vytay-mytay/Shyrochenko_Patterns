using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.ResponseModels;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces
{
    public interface IUserService
    {
        PaginationResponseModel<UserTableRowResponseModel> GetAll(PaginationRequestModel<UserTableColumn> model, bool getAdmins = false);

        UserProfileResponseModel SwitchUserActiveState(int id);

        UserProfileResponseModel SoftDeleteUser(int id);

        void HardDeleteUser(int id);

        Task<UserProfileResponseModel> EditProfileAsync(int id, UserProfileRequestModel model);

        Task<UserProfileResponseModel> GetProfileAsync(int id);

        UserProfileResponseModel DeleteAvatar(int userId);

        Task<UserDeviceResponseModel> SetDeviceToken(DeviceTokenRequestModel model, int userId);
    }
}
