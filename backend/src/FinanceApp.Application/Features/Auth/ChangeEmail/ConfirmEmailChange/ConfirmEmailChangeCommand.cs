namespace FinanceApp.Application.Features.Auth.ChangeEmail.ConfirmEmailChange;
using MediatR;

public record ConfirmEmailChangeCommand(string Token) : IRequest;
