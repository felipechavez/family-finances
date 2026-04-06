namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Shared helper for resolving a category identifier by name within a family scope.
/// </summary>
internal static class CategoryHelper
{
    /// <summary>
    /// Looks up a category by name, matching either global categories or those belonging to the specified family.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="categoryName">The category display name to search for.</param>
    /// <param name="familyId">The family scope for family-specific categories.</param>
    /// <returns>The category's unique identifier.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no matching category is found.</exception>
    internal static async Task<Guid> FindByNameAsync(Supabase.Client db, string categoryName, Guid familyId)
    {
        var category = await db.From<Category>().Where(c => c.Name == categoryName && (c.FamilyId == null || c.FamilyId == familyId)).Get();

        if (category is null || category.Model == null)
            throw new KeyNotFoundException($"Category '{categoryName}' not found.");

        return category.Model!.Id;
    }
}
