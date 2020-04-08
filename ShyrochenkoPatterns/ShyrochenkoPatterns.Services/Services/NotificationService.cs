using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Chat;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.Notifications;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.External;
using System.Linq;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services
{
    public class NotificationService : INotificationService
    {
        private IUnitOfWork _unitOfWork;
        private IFCMService _FCMService;
        private ILogger<NotificationService> _logger;
        private IMapper _mapper;

        public NotificationService(IFCMService FCMService, ILogger<NotificationService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FCMService = FCMService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task SendPushNotification(string deviceToken, PushNotification notification)
        {
            await _FCMService.SendPushNotification(deviceToken, notification);
        }

        public async Task SendPushNotification(ApplicationUser user, PushNotification notification)
        {
            // Get all active verified devices
            var devices = user.Devices?.Where(x => x.IsActive && x.IsVerified) ?? _unitOfWork.Repository<UserDevice>().Get(x => x.UserId == user.Id && x.IsActive && x.IsVerified).TagWith(nameof(SendPushNotification) + "_GetDeviceTokens").ToList();

            var badge = _unitOfWork.Repository<Chat>().Get(x => x.Users.Any(y => y.UserId == user.Id && y.IsActive))
                .TagWith(nameof(SendPushNotification) + "_GetBadge")
                .Include(x => x.Users)
                .Include(x => x.Messages)
                .FirstOrDefault().GetBadge(user.Id);

            // Send notification to all user devices
            foreach (var device in devices)
            {
                await _FCMService.SendPushNotification(device.DeviceToken, notification, badge);
            }
        }
    }
}
