using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Domain.Tests;

public class OrderTests
{
    private readonly DiscountCalculator _calculator = new();

    [Fact]
    public void AddItem_WhenCategoryAlreadyPresent_ThrowsDuplicateCategoryException()
    {
        var order = new Order();
        order.AddItem(new MenuItem { Name = "X Burger", Category = MenuCategory.Sandwich, Price = 5m });

        var act = () => order.AddItem(
            new MenuItem { Name = "X Egg", Category = MenuCategory.Sandwich, Price = 4.5m });

        act.Should().Throw<DuplicateCategoryException>()
           .Which.Category.Should().Be(MenuCategory.Sandwich);
    }

    [Fact]
    public void AddItem_WithDifferentCategories_AddsAll()
    {
        var order = new Order();

        order.AddItem(new MenuItem { Name = "X Bacon",      Category = MenuCategory.Sandwich, Price = 7m });
        order.AddItem(new MenuItem { Name = "Batata frita", Category = MenuCategory.Fries,    Price = 2m });
        order.AddItem(new MenuItem { Name = "Refrigerante", Category = MenuCategory.Drink,    Price = 2.5m });

        order.Items.Should().HaveCount(3);
    }

    [Fact]
    public void AddItem_WithNullMenuItem_ThrowsArgumentNullException()
    {
        var order = new Order();
        var act = () => order.AddItem(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Total_FullCombo_AppliesTwentyPercentDiscount()
    {
        var order = new Order();
        order.AddItem(new MenuItem { Category = MenuCategory.Sandwich, Price = 7m });
        order.AddItem(new MenuItem { Category = MenuCategory.Fries,    Price = 2m });
        order.AddItem(new MenuItem { Category = MenuCategory.Drink,    Price = 2.5m });

        order.Subtotal().Should().Be(11.5m);
        order.Total(_calculator).Should().Be(9.20m);
    }

    [Fact]
    public void Total_SandwichAndDrink_AppliesFifteenPercentDiscount()
    {
        var order = new Order();
        order.AddItem(new MenuItem { Category = MenuCategory.Sandwich, Price = 5m });
        order.AddItem(new MenuItem { Category = MenuCategory.Drink,    Price = 2.5m });

        order.Total(_calculator).Should().Be(6.375m);
    }

    [Fact]
    public void Total_SandwichAndFries_AppliesTenPercentDiscount()
    {
        var order = new Order();
        order.AddItem(new MenuItem { Category = MenuCategory.Sandwich, Price = 4.5m });
        order.AddItem(new MenuItem { Category = MenuCategory.Fries,    Price = 2m });

        order.Total(_calculator).Should().Be(5.85m);
    }

    [Fact]
    public void Total_OnlySandwich_NoDiscount()
    {
        var order = new Order();
        order.AddItem(new MenuItem { Category = MenuCategory.Sandwich, Price = 5m });

        order.Total(_calculator).Should().Be(5m);
    }
}
