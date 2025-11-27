using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Quartz;

namespace Hortifia.Infrastructure.Jobs;

public class WateringNotificationJob(INotificationService notificationService,
                                    IPlantsRepository plantsRepository,
                                    IUserDeviceTokenRepository userDeviceTokenRepository) : IJob
{
    internal const string Name = nameof(WateringNotificationJob);
    public async Task Execute(IJobExecutionContext context)
    {
        var userId = context.MergedJobDataMap.GetString("UserId")!;

        var plantsToWater = await plantsRepository.GetPlantsToNotificationAsync(userId);

        if (!plantsToWater.Any())
        {
            return;
        }

        var deviceTokens = await userDeviceTokenRepository.GetTokensByUserIdAsync(userId);

        if (deviceTokens.Count == 0)
        {
            return;
        }

        var notificationContent = string.Empty;
        var notificationTitle = string.Empty;

        if (plantsToWater.Count() == 1)
        {
            notificationTitle = "Time to water your plant!";
            notificationContent = $"Don't forget to water {plantsToWater.First().Name}.";
        }
        else
        {
            var plantNames = string.Join(", ", plantsToWater.Select(p => p.Name));
            notificationTitle = "Time to water your plant!";
            notificationContent = $"Don't forget to water {plantNames}.";
        }
        foreach (var deviceToken in deviceTokens)
        {
            await notificationService.SendNotificationAsync(
                userId: userId,
                deviceToken: deviceToken,
                title: notificationTitle,
                body: notificationContent);
        }

    }
}
