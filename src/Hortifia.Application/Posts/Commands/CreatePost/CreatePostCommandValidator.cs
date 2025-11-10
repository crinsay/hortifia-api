using FluentValidation;
using Hortifia.Application.Extensions;

namespace Hortifia.Application.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(p => p.Title)
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters.");

        RuleFor(p => p.Content)
            .MaximumLength(4096)
            .WithMessage("Content cannot exceed 4096 characters.");

        RuleFor(p => p.Hashtags)
            .Must(hashtags => hashtags.Count <= 10)
            .WithMessage("You can specify up to 10 hashtags.")
            .Must(hashtags => !hashtags.Any(h => h.Length > 20))
            .WithMessage("Hashtag length cannot exceed 20 characters.");

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
