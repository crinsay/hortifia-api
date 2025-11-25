using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlants;

public class GetPlantsQueryHandler(IPlantsRepository plantsRepository,
    IUserContext userContext,
    IBlobStorageService blobStorageService) : IRequestHandler<GetPlantsQuery, Result<IEnumerable<PlantListDto>>>
{
    public async Task<Result<IEnumerable<PlantListDto>>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var plants = await plantsRepository.GetDtosByUserIdAsync(currentUser.Id!, 
            request.SearchPhrase, 
            request.PageNumber, 
            request.PageSize,
            request.OnlyFavourites,
            request.LimitToFour,
            request.OnlyPlantsInNeed);

        foreach (var plant in plants)
        {
            var imgBlobName = plant.ImgUrl;
            if (imgBlobName is not null)
            {
                plant.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(imgBlobName);
            }
        }

        return Result<IEnumerable<PlantListDto>>.Success(plants);
    }
}
