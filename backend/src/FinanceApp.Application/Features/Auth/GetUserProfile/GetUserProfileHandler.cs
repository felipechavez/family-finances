namespace FinanceApp.Application.Features.Auth.GetUserProfile;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;

public class GetUserProfileHandler(Client supabase)
    : IRequestHandler<GetUserProfileQuery, UserProfileResult>
{
    public async Task<UserProfileResult> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var resp = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();

        var user = resp.Model
            ?? throw new AppException(LocalizationKeys.Auth_UserNotFound, 404);

        return new UserProfileResult(user.TwoFactorEnabled, user.DailySummaryEnabled);
    }
}
