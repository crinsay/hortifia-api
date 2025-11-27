namespace Hortifia.Application.Common.Interfaces.Services;

public interface IQuartzSchedulerService
{
    Task ScheduleWateringNotificationForUserAsync(string userId, DateTime notificationDate);
}
