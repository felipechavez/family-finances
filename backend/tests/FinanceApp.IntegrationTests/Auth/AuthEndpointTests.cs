namespace FinanceApp.IntegrationTests.Auth;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AuthEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var payload = new { Name = "Test User", Email = "test@test.com", Password = "password123" };
        var response = await _client.PostAsJsonAsync("/auth/register", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var payload = new { Email = "nobody@test.com", Password = "wrong" };
        var response = await _client.PostAsJsonAsync("/auth/login", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
