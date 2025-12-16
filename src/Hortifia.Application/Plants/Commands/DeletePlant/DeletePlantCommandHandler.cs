using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.DeletePlant;

public class DeletePlantCommandHandler(IPlantsRepository plantsRepository,
    ILogger<DeletePlantCommandHandler> logger,
    IUserContext userContext,
    IBlobStorageService blobStorageService,
    IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService) : IRequestHandler<DeletePlantCommand, Result>
{
    public async Task<Result> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var plantId = request.PlantId;

        var plant = await plantsRepository.GetByIdAsync(plantId);

        if (plant is null)
        {
            logger.LogWarning("Plant with ID {PlantId} not found.", plantId);
            return Result.Failure("Plant not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, plant, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Plant with id {roomId} does not belong to the current user.", plantId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result.Failure("Plant not found.");
        }

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await plantsRepository.DeleteAsync(plant);

            var plantImgBlobName = plant.ImgBlobName;
            if (plantImgBlobName is not null)
            {
                await blobStorageService.DeleteBlobAsync(plantImgBlobName);
            }
        });

        return Result.Success();
    }
}
