using FirebaseAdmin.Messaging;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Hortifia.Infrastructure.Services.Firebase;

public class NotificationService(IUserDeviceTokenRepository userDeviceTokenRepository,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendNotificationAsync(string userId, string deviceToken, string title, string body)
    {
        var message = new Message
        {
            Token = deviceToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            }
        };

        try
        {
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            logger.LogInformation("Successfully sent notification to user {UserId} with token {DeviceToken}. Response: {Response}", userId, deviceToken, response);
        }
        catch (FirebaseMessagingException ex)
        {
            logger.LogError(ex, "Firebase messaging error when sending notification to user {UserId} with token {DeviceToken}", userId, deviceToken);

            if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument ||
                ex.MessagingErrorCode == MessagingErrorCode.SenderIdMismatch)
            {
                var token = UserDeviceToken.Create(userId, deviceToken);
                await userDeviceTokenRepository.DeleteAsync(token);
                logger.LogInformation("Removed invalid device token for user {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending notification to user {UserId} with token {DeviceToken}", userId, deviceToken);
        }
    }
}
