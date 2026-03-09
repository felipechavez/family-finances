namespace FinanceApp.Application.Features.Auth.Register;
using MediatR;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<RegisterResult>;
public record RegisterResult(Guid UserId, string Email);
