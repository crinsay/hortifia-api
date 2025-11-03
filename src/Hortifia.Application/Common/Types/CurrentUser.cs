namespace Hortifia.Application.Common.Types;
public record CurrentUser
{
    public string? Id { get; init; }
    public string? NickName { get; init; }
}
