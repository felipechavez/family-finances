namespace FinanceApp.Application.Features.Auth.ChangeEmail.InitiateEmailChange;
using MediatR;

public record InitiateEmailChangeCommand(Guid UserId, string NewEmail) : IRequest;
