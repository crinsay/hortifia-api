using System.Text.Json.Serialization;

namespace Hortifia.Application.Location.Responses;

public class CityNameApiResponse
{
    [JsonPropertyName("address")]
    public Dictionary<string, string>? Address { get; init; }

    public string? CityName 
    { 
        get
        {
            if (Address is null)
            {
                return null;
            }

            if (field is not null)
            {
                return field;
            }

            IEnumerable<string> cityKeys = ["city", "town", "region", "village", "municipality", "hamlet", "locality", "suburb"];
            foreach(var key in cityKeys)
            {
                if (Address.TryGetValue(key, out var cityName))
                {
                    field = cityName;
                    return cityName;
                }
            }
            return null;
        }
    }
}
