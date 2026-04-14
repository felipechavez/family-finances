namespace FinanceApp.API.Endpoints;

using FinanceApp.Domain.Entities;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;

/// <summary>
/// Shared helper for resolving a category identifier by name within a family scope.
/// </summary>
internal static class CategoryHelper
{
    /// <summary>
    /// Looks up a category by name, matching either global categories or those belonging to the specified family.
    /// </summary>
    /// <param name="db">The Supabase client.</param>
    /// <param name="categoryName">The category display name to search for.</param>
    /// <param name="familyId">The family scope for family-specific categories.</param>
    /// <returns>The category's unique identifier.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no matching category is found.</exception>
    internal static async Task<Guid> FindByNameAsync(Supabase.Client db, string categoryName, Guid familyId)
    {
        if (db is null) throw new ArgumentNullException(nameof(db));
        if (string.IsNullOrWhiteSpace(categoryName)) throw new ArgumentException("El nombre de categoría no puede estar vacío.", nameof(categoryName));

        // name=eq.X AND (family_id IS NULL OR family_id=X)

        var filters = new List<IPostgrestQueryFilter>
        {
            new QueryFilter("family_id", Constants.Operator.Is, QueryFilter.NullVal),
            new QueryFilter("family_id", Constants.Operator.Equals, familyId)
        };
        var result = await db.From<Category>()
            .Filter("name", Constants.Operator.Equals, categoryName)
            .Or(filters)
            .Get();

        if (result?.Models == null || result.Models.Count == 0)
            throw new KeyNotFoundException($"Category '{categoryName}' not found.");

        return result.Models[0].Id;
    }
}
