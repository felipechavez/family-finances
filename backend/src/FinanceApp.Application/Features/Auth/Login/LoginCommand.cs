namespace FinanceApp.Application.Features.Auth.Login;
using MediatR;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
public record LoginResult(string Token, Guid UserId, string Name, string Email, Guid? FamilyId);
