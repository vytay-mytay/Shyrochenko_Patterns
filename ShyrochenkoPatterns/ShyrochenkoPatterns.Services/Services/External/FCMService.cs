using FcmSharp;
using FcmSharp.Requests;
using FcmSharp.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Logging;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.Notifications;
using ShyrochenkoPatterns.Services.Interfaces.External;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.External
{
    public class FCMService : IFCMService
    {
        private IConfiguration _configuration;
        private IHostingEnvironment _environment;
        private IUnitOfWork _unitOfWork;
        private ILogger<FCMService> _logger;

        public FCMService(IConfiguration configuration, IHostingEnvironment environment, IUnitOfWork unitOfWork, ILogger<FCMService> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> SendPushNotification(string deviceToken, PushNotification notification, int badge = 0, bool testMode = false)
        {
            bool sent = false;

            // The Message should be sent to the News Topic:
            var message = new FcmMessage()
            {
                ValidateOnly = false,
                Message = new Message
                {
                    Token = deviceToken,
                    Data = notification.Data != null ? notification.Data.GetDictionary() : null,
                    AndroidConfig = new AndroidConfig
                    {
                        CollapseKey = notification.Data.ThreadId,
                        Data = notification.Data != null ? notification.Data.GetAndroidDictionary(notification.Title, notification.Body, badge) : null,
                    },
                    ApnsConfig = new ApnsConfig
                    {
                        Payload = new ApnsConfigPayload
                        {
                            Aps = new Aps
                            {
                                Alert = new ApsAlert
                                {
                                    Title = notification.Title,
                                    Body = notification.Body
                                },
                                Badge = badge,
                                Sound = "default",
                                CustomData = notification.Data != null ? notification.Data.GetData() : null,
                                ThreadId = notification.Data.ThreadId
                            }
                        }
                    }
                }
            };
            
            var jsonBody = JsonConvert.SerializeObject(message);

            PushNotificationLog log = new PushNotificationLog
            {
                DeviceToken = deviceToken,
                Title = notification.Title,
                Body = notification.Body,
                DataJSON = jsonBody,
                CreatedAt = DateTime.UtcNow,
                Status = SendingStatus.Failed
            };

            try
            {
                // Read the Service Account Key from a File, which is not under Version Control:
                var settings = FileBasedFcmClientSettings.CreateFromFile(_configuration["FCM:ProjectId"], _configuration["FCM:KeyPath"]);

                // Construct the Client:
                using (var client = new FcmClient(settings))
                {
                    // Finally send the Message and wait for the Result:
                    CancellationTokenSource cts = new CancellationTokenSource();

                    // Send the Message and wait synchronously:
                    var result = await client.SendAsync(message, cts.Token);

                    log.Status = SendingStatus.Success;
                    sent = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"FCM Service: {ex}");
                sent = false;

                if (testMode)
                    throw;
            }
            finally
            {
                _unitOfWork.Repository<PushNotificationLog>().Insert(log);
                _unitOfWork.SaveChanges();
            }

            return sent;
        }
    }
}
