using FluentValidation;
using Hortifia.Application.Extensions;

namespace Hortifia.Application.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(p => p.Title)
            .Length(1, 100)
            .WithMessage("Title must have length between 1 and 100 characters.");

        RuleFor(p => p.Content)
            .Length(1, 4096)
            .WithMessage("Content must have length between 1 and 4096 characters.");

        RuleFor(p => p.Hashtags)
            .Must(hashtags => hashtags.Count <= 10)
            .WithMessage("You can specify up to 10 hashtags.")
            .Must(hashtags => !hashtags.Any(h => h.Length > 20))
            .WithMessage("Hashtag length cannot exceed 20 characters.")
            .Must(hashtags =>
            {
                var uniqueHashtags = hashtags
                    .Select(h => h.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                return uniqueHashtags.Count == hashtags.Count;
            })
            .WithMessage("All hashtags must be unique.");

        RuleFor(c => c.Img)
            .Custom((value, context) =>
            {
                 if (value is not null)
                 {
                     if (!value.IsImage())
                     {
                         context.AddFailure("Img", "You must pass an image as a file.");
                     }

                     if (value.FileName.Length > 256)
                     {
                        context.AddFailure("Img", "Img file name is too long. Maximum is 256 characters.");
                     }
                 }
            });
    }
}
