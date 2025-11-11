using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlantApiDataById;

public class GetPlantApiDataByIdQueryHandler(IPermapeopleApiService apiService,
    ILogger<GetPlantApiDataByIdQueryHandler> logger) : IRequestHandler<GetPlantApiDataByIdQuery, Result<PlantApiDto>>
{
    public async Task<Result<PlantApiDto>> Handle(GetPlantApiDataByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.PlantApiId <= 0)
        {
            logger.LogWarning("Invalid PlantApiId {PlantApiId} provided.", request.PlantApiId);
            return Result<PlantApiDto>.Failure("Invalid PlantApiId provided.");
        }

        var plantApi = await apiService.GetPlantByIdAsync(request.PlantApiId);

        if (plantApi is null)
        {
            logger.LogWarning("Plant with PlantApiId {PlantApiId} not found in external API.", request.PlantApiId);
            return Result<PlantApiDto>.Failure($"Plant with PlantApiId {request.PlantApiId} not found.");
        }

        return Result<PlantApiDto>.Success(plantApi);
    }
}