namespace FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;

public class CreateAccountHandler(IAppDbContext db) : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = Account.Create(request.FamilyId, request.Name, request.Type, request.InitialBalance);
        db.Accounts.Add(account);
        await db.SaveChangesAsync(cancellationToken);
        return account.Id;
    }
}
