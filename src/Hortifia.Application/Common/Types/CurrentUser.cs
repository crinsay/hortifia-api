namespace Hortifia.Application.Common.Types;

public sealed record CurrentUser(string? Id, bool IsAuthenticated = false, TimeOnly PrefferedNotificationTime = default);
