using FluentAssertions;
using NetArchTest.Rules;

namespace GoodHamburger.Api.Tests.Architecture;

public class ArchitectureTests
{
    private const string Domain = "GoodHamburger.Domain";
    private const string Application = "GoodHamburger.Application";
    private const string Infrastructure = "GoodHamburger.Infrastructure";
    private const string Api = "GoodHamburger.Api";

    [Fact]
    public void Domain_ShouldNotDependOnAnyOtherLayer()
    {
        var result = Types.InAssembly(typeof(Domain.Entities.Order).Assembly)
            .Should()
            .NotHaveDependencyOnAny(Application, Infrastructure, Api)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Domain must remain pure and have no outward dependencies. Failing types: " +
            string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>()));
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(Application.Orders.IOrderService).Assembly)
            .Should()
            .NotHaveDependencyOnAny(Infrastructure, Api)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Application must depend on Domain only. Failing types: " +
            string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>()));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(typeof(Infrastructure.Persistence.AppDbContext).Assembly)
            .Should()
            .NotHaveDependencyOn(Api)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Infrastructure must not depend on Api. Failing types: " +
            string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>()));
    }
}
