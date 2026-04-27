using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Domain.Tests;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _sut = new();

    [Fact]
    public void Calculate_WithSandwichFriesAndDrink_Returns20Percent()
    {
        var items = new List<OrderItem>
        {
            new() { Category = MenuCategory.Sandwich },
            new() { Category = MenuCategory.Fries },
            new() { Category = MenuCategory.Drink }
        };

        _sut.Calculate(items).Should().Be(0.20m);
    }

    [Fact]
    public void Calculate_WithSandwichAndDrink_Returns15Percent()
    {
        var items = new List<OrderItem>
        {
            new() { Category = MenuCategory.Sandwich },
            new() { Category = MenuCategory.Drink }
        };

        _sut.Calculate(items).Should().Be(0.15m);
    }

    [Fact]
    public void Calculate_WithSandwichAndFries_Returns10Percent()
    {
        var items = new List<OrderItem>
        {
            new() { Category = MenuCategory.Sandwich },
            new() { Category = MenuCategory.Fries }
        };

        _sut.Calculate(items).Should().Be(0.10m);
    }

    [Fact]
    public void Calculate_WithOnlySandwich_ReturnsZero()
    {
        var items = new List<OrderItem>
        {
            new() { Category = MenuCategory.Sandwich }
        };

        _sut.Calculate(items).Should().Be(0m);
    }

    [Fact]
    public void Calculate_WithFriesAndDrink_ReturnsZero()
    {
        var items = new List<OrderItem>
        {
            new() { Category = MenuCategory.Fries },
            new() { Category = MenuCategory.Drink }
        };

        _sut.Calculate(items).Should().Be(0m);
    }

    [Fact]
    public void Calculate_WithEmptyList_ReturnsZero()
    {
        _sut.Calculate(new List<OrderItem>()).Should().Be(0m);
    }

    [Fact]
    public void Calculate_WithNullItems_Throws()
    {
        var act = () => _sut.Calculate(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
