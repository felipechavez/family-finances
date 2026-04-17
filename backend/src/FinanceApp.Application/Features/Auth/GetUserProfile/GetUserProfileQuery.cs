namespace FinanceApp.Application.Features.Auth.GetUserProfile;
using MediatR;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileResult>;

public record UserProfileResult(bool TwoFactorEnabled, bool DailySummaryEnabled);
