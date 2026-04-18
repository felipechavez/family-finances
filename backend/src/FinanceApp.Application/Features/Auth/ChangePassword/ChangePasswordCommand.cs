namespace FinanceApp.Application.Features.Auth.ChangePassword;
using MediatR;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword, string ConfirmNewPassword) : IRequest;
