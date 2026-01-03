using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Infrastructure.Jobs;
using Quartz;

namespace Hortifia.Infrastructure.Services.Quartz;

public class QuartzSchedulerService(ISchedulerFactory schedulerFactory) : IQuartzSchedulerService
{
    private readonly IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

    public async Task ScheduleWateringNotificationForUserAsync(string userId, DateTime notificationDate)
    {
        var jobKey = new JobKey($"WateringJob-{userId}-{notificationDate:yyyyMMdd}");
        if (await _scheduler.CheckExists(jobKey)) return;

        var jobData = new JobDataMap
        {
            { "UserId", userId }
        };

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
