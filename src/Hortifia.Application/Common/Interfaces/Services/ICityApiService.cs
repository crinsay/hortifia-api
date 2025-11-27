namespace Hortifia.Application.Common.Interfaces.Services;

public interface ICityApiService
{
    Task<string?> GetCityNameAsync(double latitude, double longitude);
}
