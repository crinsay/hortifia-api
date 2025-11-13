using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.DeletePlant;

public class DeletePlantCommandHandler(IPlantsRepository plantsRepository,
    ILogger<DeletePlantCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<DeletePlantCommand, Result>
{
    public async Task<Result> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var plant = await plantsRepository.GetByIdAsync(request.PlantId);

        if (plant is null || plant.UserId != currentUser.Id)
        {
            logger.LogWarning("Plant with ID {PlantId} not found or does not belong to the user.", request.PlantId);
            return Result.Failure("Plant not found.");
        }

        await plantsRepository.DeleteAsync(plant);

        return Result.Success();
    }
}
