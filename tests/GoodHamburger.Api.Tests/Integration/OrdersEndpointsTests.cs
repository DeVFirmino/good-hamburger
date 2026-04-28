using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GoodHamburger.Application.Menu.Dtos;
using GoodHamburger.Application.Orders.Dtos;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Api.Tests.Integration;

public class OrdersEndpointsTests : IClassFixture<GoodHamburgerWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersEndpointsTests(GoodHamburgerWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMenu_ReturnsSeededItems()
    {
        var items = await _client.GetFromJsonAsync<List<MenuItemResponse>>("/api/menu");

        items.Should().NotBeNull();
        items!.Should().HaveCount(5);
        items.Should().Contain(i => i.Name == "X Burger" && i.Category == MenuCategory.Sandwich);
        items.Should().Contain(i => i.Name == "Refrigerante" && i.Category == MenuCategory.Drink);
    }

    [Fact]
    public async Task CreateOrder_FullCombo_AppliesTwentyPercentDiscount()
    {
        var menu = await _client.GetFromJsonAsync<List<MenuItemResponse>>("/api/menu");
        int sandwich = menu!.First(i => i.Category == MenuCategory.Sandwich).Id;
        int fries    = menu.First(i => i.Category == MenuCategory.Fries).Id;
        int drink    = menu.First(i => i.Category == MenuCategory.Drink).Id;

        var response = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest { MenuItemIds = new List<int> { sandwich, fries, drink } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        order!.Items.Should().HaveCount(3);
        order.DiscountRate.Should().Be(0.20m);
        order.Total.Should().Be(order.Subtotal - (order.Subtotal * 0.20m));
    }

    [Fact]
    public async Task CreateOrder_DuplicateCategory_Returns422()
    {
        var menu = await _client.GetFromJsonAsync<List<MenuItemResponse>>("/api/menu");
        var sandwiches = menu!.Where(i => i.Category == MenuCategory.Sandwich).Take(2).Select(i => i.Id).ToList();

        var response = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest { MenuItemIds = sandwiches });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("ORDER_DUPLICATE_CATEGORY");
    }

    [Fact]
    public async Task CreateOrder_EmptyList_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest { MenuItemIds = new List<int>() });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_InvalidMenuId_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest { MenuItemIds = new List<int> { 9999 } });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("MENU_ITEM_NOT_FOUND");
    }

    [Fact]
    public async Task GetOrderById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrder_ReplacesItems()
    {
        var menu = await _client.GetFromJsonAsync<List<MenuItemResponse>>("/api/menu");
        int sandwich = menu!.First(i => i.Category == MenuCategory.Sandwich).Id;
        int fries    = menu.First(i => i.Category == MenuCategory.Fries).Id;

        var created = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest { MenuItemIds = new List<int> { sandwich } });
        var order = (await created.Content.ReadFromJsonAsync<OrderResponse>())!;

        var update = await _client.PutAsJsonAsync($"/api/orders/{order.Id}",
            new UpdateOrderRequest { MenuItemIds = new List<int> { sandwich, fries } });
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = (await update.Content.ReadFromJsonAsync<OrderResponse>())!;
        updated.Items.Should().HaveCount(2);
        updated.DiscountRate.Should().Be(0.10m);
    }

    [Fact]
    public async Task DeleteOrder_RemovesIt()
    {
        var menu = await _client.GetFromJsonAsync<List<MenuItemResponse>>("/api/menu");
        int sandwich = menu!.First(i => i.Category == MenuCategory.Sandwich).Id;

        var created = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest { MenuItemIds = new List<int> { sandwich } });
        var order = (await created.Content.ReadFromJsonAsync<OrderResponse>())!;

        var delete = await _client.DeleteAsync($"/api/orders/{order.Id}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var fetch = await _client.GetAsync($"/api/orders/{order.Id}");
        fetch.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
