using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Infrastructure.Jobs;
using Quartz;

namespace Hortifia.Infrastructure.Services.Quartz;

public class QuartzSchedulerService(ISchedulerFactory schedulerFactory) : IQuartzSchedulerService
{
    private readonly IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

    // Use this method to schedule a watering notification for a user at a specific date and time
    public async Task ScheduleWateringNotificationForUserAsync(string userId, DateTime notificationDate)
    {
        // Set job name and check if job already exists
        var jobKey = new JobKey($"WateringJob-{userId}-{notificationDate:yyyyMMdd}");
        if (await _scheduler.CheckExists(jobKey)) return;

        // Set job data
        var jobData = new JobDataMap
        {
            { "UserId", userId }
        };

        // Create job and trigger
        var job = JobBuilder.Create<WateringNotificationJob>()
            .WithIdentity(jobKey)
            .UsingJobData(jobData)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"WateringTrigger-{userId}-{notificationDate:yyyyMMdd}")
            .StartAt(notificationDate)
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }
}
