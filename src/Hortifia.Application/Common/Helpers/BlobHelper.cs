using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Helpers;

public static class BlobHelper
{
    public static Result<string> GetBlobName<TParent>(int parentId, string originalFileName)
    {
        var blobFolderResult = GetBlobFolderName(typeof(TParent));
        if (!blobFolderResult.IsSuccess)
        {
            return Result<string>.Failure(blobFolderResult.ErrorMessage!);
        }

        return Result<string>.Success($"{blobFolderResult.Value}/{parentId}-{originalFileName}");
    }

    private static Result<string> GetBlobFolderName(Type blobParent)
    {
        return blobParent switch
        {
            var parent when parent == typeof(Post) => Result<string>.Success(BlobFolderNames.Posts),
            _ => Result<string>.Failure("Unknown blob parent.")
        };
    }
}
