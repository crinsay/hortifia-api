using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlants;

public class GetPlantsQueryHandler(IPlantsRepository plantsRepository,
    ILogger<GetPlantsQueryHandler> logger,
    IUserContext userContext) : IRequestHandler<GetPlantsQuery, Result<IEnumerable<PlantListDto>>>
{
    public async Task<Result<IEnumerable<PlantListDto>>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var plants = await plantsRepository.GetAllDtosByUserIdAsync(currentUser.Id!, request.SearchPhrase, request.PageNumber, request.PageSize);

        if (plants is null) 
        {
            logger.LogInformation("No plants found for user {UserId}.", currentUser.Id);
            return Result<IEnumerable<PlantListDto>>.Failure("No plants found.");
        }

        foreach (var plant in plants) 
        {
            var daysToNextWatering = (plant.ExpectedWateringDate - DateTime.UtcNow).TotalDays;

            if (daysToNextWatering < 0)
            {
                daysToNextWatering = 0;
            }

            plant.DaysToNextWatering = (int)Math.Ceiling(daysToNextWatering);

            if (plant.WateringStatus < 0)
            {
                plant.WateringStatus = 0;
            }
        }

        return Result<IEnumerable<PlantListDto>>.Success(plants);
    }
}
