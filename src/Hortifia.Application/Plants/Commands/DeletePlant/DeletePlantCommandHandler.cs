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

        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.Id))
        {
            logger.LogWarning("Unauthorized attempt to delete a room.");
            return Result.Failure("User is not authenticated.");
        }

        var plant = await plantsRepository.GetByIdAsync(request.PlantId);

        if (plant == null || plant.UserId != currentUser.Id)
        {
            logger.LogWarning("Plant with ID {PlantId} not found or does not belong to the user.", request.PlantId);
            return Result.Failure("Plant not found or access denied.");
        }

        await plantsRepository.DeleteAsync(plant);

        return Result.Success();
    }
}
