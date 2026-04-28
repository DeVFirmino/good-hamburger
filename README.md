# Good Hamburger API

REST API for managing orders at a small burger restaurant. Built as a .NET technical
challenge: full CRUD over orders, a menu catalog, and a discount engine that applies a
combo rule when an order contains items from specific categories.

## Discount rules

| Categories present                | Discount |
| --------------------------------- | -------- |
| Sandwich + Fries + Drink          | 20%      |
| Sandwich + Drink                  | 15%      |
| Sandwich + Fries                  | 10%      |
| Anything else                     | 0%       |

An order may contain at most one item per category.

## Menu

| Item            | Category | Price (R$) |
| --------------- | -------- | ---------- |
| X Burger        | Sandwich | 5.00       |
| X Egg           | Sandwich | 4.50       |
| X Bacon         | Sandwich | 7.00       |
| Batata frita    | Fries    | 2.00       |
| Refrigerante    | Drink    | 2.50       |

## Stack

- .NET 10, ASP.NET Core Web API (Controllers)
- EF Core 10 + SQLite (file-based at runtime, in-memory for tests)
- FluentValidation for request validation
- Serilog for structured console logging
- Swagger / Swashbuckle for API exploration
- xUnit + FluentAssertions + NetArchTest for testing

## Solution layout

```
src/
  GoodHamburger.Domain/          // Entities, value rules, exceptions, DiscountCalculator
  GoodHamburger.Application/     // Services, DTOs, validators, abstractions (repos, UoW)
  GoodHamburger.Infrastructure/  // EF Core DbContext, configurations, repositories, seeder
  GoodHamburger.Api/             // Controllers, exception middleware, Program.cs

tests/
  GoodHamburger.Domain.Tests/    // Pure domain tests (discount rules, order rules)
  GoodHamburger.Api.Tests/       // Integration tests via WebApplicationFactory + arch tests
```

Dependencies flow inward: `Api → Infrastructure → Application → Domain`. Domain has no
references to other layers; this is verified by `ArchitectureTests`.

## Running locally

Requirements: .NET 10 SDK (the version is pinned in `global.json`).

```bash
dotnet restore
dotnet run --project src/GoodHamburger.Api
```

The API starts on the URL printed to the console (default `http://localhost:5163`).
On startup it applies the EF migration to a SQLite file (`goodhamburger.db`) and seeds
the menu. Swagger UI is available at `/swagger` in the Development environment.

## Endpoints

| Method | Path                  | Description                          |
| ------ | --------------------- | ------------------------------------ |
| GET    | `/api/menu`           | List menu items                      |
| POST   | `/api/orders`         | Create an order                      |
| GET    | `/api/orders`         | List orders (newest first)           |
| GET    | `/api/orders/{id}`    | Get a single order                   |
| PUT    | `/api/orders/{id}`    | Replace the items of an order        |
| DELETE | `/api/orders/{id}`    | Delete an order                      |

### Sample requests

Create a full-combo order (X Burger + Batata frita + Refrigerante):

```bash
curl -X POST http://localhost:5163/api/orders \
  -H "Content-Type: application/json" \
  -d '{ "menuItemIds": [1, 4, 5] }'
```

Response (HTTP 201):

```json
{
  "id": "8d3028b8-...-...",
  "createdAt": "2026-04-28T14:59:21.075Z",
  "items": [
    { "menuItemId": 1, "name": "X Burger",     "category": 1, "price": 5.00 },
    { "menuItemId": 4, "name": "Batata frita", "category": 2, "price": 2.00 },
    { "menuItemId": 5, "name": "Refrigerante", "category": 3, "price": 2.50 }
  ],
  "subtotal": 9.50,
  "discountRate": 0.20,
  "total": 7.60
}
```

## Error model

Errors are returned as RFC 7807 ProblemDetails. Each business error carries a stable
`code` for clients to branch on:

| Status | Code                        | Cause                                       |
| ------ | --------------------------- | ------------------------------------------- |
| 400    | `VALIDATION_FAILED`         | FluentValidation rule violated              |
| 400    | `MENU_ITEM_NOT_FOUND`       | Referenced menu item does not exist         |
| 404    | `ORDER_NOT_FOUND`           | Order id not found                          |
| 422    | `ORDER_DUPLICATE_CATEGORY`  | More than one item from the same category   |
| 500    | `INTERNAL_ERROR`            | Unhandled error                             |

## Tests

```bash
dotnet test
```

Test suites:

- **`GoodHamburger.Domain.Tests`** – unit tests for the discount rules and order
  invariants (no external dependencies).
- **`GoodHamburger.Api.Tests`** – integration tests using
  `WebApplicationFactory<Program>` and SQLite in-memory; architecture tests using
  `NetArchTest` to enforce the dependency direction between layers.

## Design notes

- **Aggregate root.** `Order` is the consistency boundary: it owns its `OrderItem`s
  and enforces the "one item per category" invariant inside `AddItem`.
- **Snapshot on add.** `OrderItem` copies `Name`, `Category`, and `Price` from the
  `MenuItem` at the moment of adding. Future menu price changes do not retroactively
  alter past orders.
- **Stateless calculator.** `DiscountCalculator` exposes a single `Calculate` method
  and is registered as a singleton. All rates are named constants — no magic numbers.
- **Repository + Unit of Work.** Application code depends on `IOrderRepository`,
  `IMenuItemRepository`, and `IUnitOfWork`. Infrastructure provides the EF Core
  implementations; this keeps the application layer free of EF references and makes
  swapping persistence trivial.
- **Stable error codes.** Every domain exception carries a `Code` that the
  exception-handling middleware surfaces in the ProblemDetails payload, decoupling
  client logic from human-readable messages.

## Branching and commits

Each layer was developed on a feature branch with a single, focused commit:

```
feat/domain-core      — Domain entities, exceptions, DiscountCalculator
feat/application      — Services, DTOs, validators, abstractions
feat/infrastructure   — EF Core SQLite, repositories, seeder, migration
feat/api              — Controllers, exception middleware, Program.cs
chore/tests-and-docs  — Integration tests, architecture tests, README
```
