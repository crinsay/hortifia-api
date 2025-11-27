using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Dtos;

public class DailyWeatherInfoDto
{
    [JsonPropertyName("temperature_2m_mean")]
    public IEnumerable<float>? Temperatures { get; init; }
}
