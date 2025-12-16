using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.UpdateIsFavourite;

public class UpdateIsFavouriteCommandHandler(IPlantsRepository plantsRepository,
    ILogger<UpdateIsFavouriteCommandHandler> logger,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateIsFavouriteCommand, Result>
{
    public async Task<Result> Handle(UpdateIsFavouriteCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var plantId = request.PlantId;

        var plant = await plantsRepository.GetByIdAsync(plantId);

        if (plant is null)
        {
            logger.LogWarning("Plant with ID {PlantId} not found for user {UserId}.", plantId, currentUser.Id);
            return Result.Failure("Plant not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, plant, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Plant with id {roomId} does not belong to the current user.", plantId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result.Failure("Plant not found.");
        }

        plant.ToggleFavourite();

        await plantsRepository.SaveChangesAsync();

        return Result.Success();
    }
}
