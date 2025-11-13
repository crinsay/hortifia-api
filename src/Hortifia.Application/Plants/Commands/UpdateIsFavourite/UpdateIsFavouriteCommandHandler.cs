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

        var plant = await plantsRepository.GetByIdAsync(request.PlantId);

        if (plant is null || plant.UserId != currentUser.Id)
        {
            logger.LogWarning("Plant with ID {PlantId} not found for user {UserId}.", request.PlantId, currentUser.Id);
            return Result.Failure("Plant not found.");
        }

        plant.ToggleFavourite();

        await plantsRepository.SaveChangesAsync();

        return Result.Success();
    }
}
