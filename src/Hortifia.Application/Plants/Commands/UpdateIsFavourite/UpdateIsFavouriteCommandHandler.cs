using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.UpdateIsFavourite;

public class UpdateIsFavouriteCommandHandler(IPlantsRepository plantsRepository,
    ILogger<UpdateIsFavouriteCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<UpdateIsFavouriteCommand, Result>
{
    public async Task<Result> Handle(UpdateIsFavouriteCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.Id))
        {
            logger.LogWarning("Unauthorized attempt to update a room.");
            return Result.Failure("User is not authenticated.");
        }

        var plant = await plantsRepository.GetByIdAsync(request.Id);

        if (plant == null || plant.UserId != currentUser.Id)
        {
            logger.LogWarning("Plant with ID {PlantId} not found for user {UserId}.", request.Id, currentUser.Id);
            return Result.Failure("Plant not found.");
        }

        plant.UpdateIsFavourite();

        await plantsRepository.SaveChangesAsync();

        return Result.Success();
    }
}
