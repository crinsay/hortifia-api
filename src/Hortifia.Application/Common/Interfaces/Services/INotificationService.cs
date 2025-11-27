namespace Hortifia.Application.Common.Interfaces.Services;

public interface INotificationService
{
    Task SendNotificationAsync(string userId, string deviceToken, string title, string body);
}