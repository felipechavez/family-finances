using System.IdentityModel.Tokens.Jwt;

namespace FinanceApp.Domain.Common
{
    public static class FamilyClaims
    {
        public const string FamilyId = "family_id";
        public const string UserId = JwtRegisteredClaimNames.Sub;
        public const string Email = JwtRegisteredClaimNames.Email;
        public const string Name = JwtRegisteredClaimNames.Name;
        public const string Jti = JwtRegisteredClaimNames.Jti;
    }
}
