# Admin Orders Page — Infinite Scroll + Expandable Product Rows (standalone models/SPs)

## Context

This is a standalone admin feature: a new page listing all orders with infinite scroll (lazy-loaded pages as the admin scrolls down), where each row has a triangle toggle to expand/collapse a nested product list for that order (lazy-loaded from the DB only when first expanded, not preloaded).

Per explicit user decisions:
- New, slim DTOs/Models/SP-models are used instead of reusing `OrderDetailsDTO`/`OrderItemDetailsDTO` (those carry unneeded fields like `Username`, `FullName`, `ProductDescription`, `BrandName`, `CategoryName`, `ModifiedAt`, `AddressId`, `UserId`).
- Two new stored procedures are required (SQL provided below, to be run manually by the user in SSMS — no `/Scripts` folder exists in the repo currently, so no `.sql` file needs to be added to source control).
- No new services/controllers — reuse/extend the **existing** `IOrderService`/`OrderService`, `IOrderFrontendService`/`OrderFrontendService`, and `OrderController`.
- Orders list uses Blazor's built-in `<Virtualize>` component for infinite scroll (`ItemsProvider` pattern), not manual JS IntersectionObserver.
- Since `<Virtualize>` doesn't integrate with `MudTable`'s row/column model, the orders table is rendered as a plain HTML `<table>` (styled to visually match the rest of the app), with MudBlazor atoms (`MudChip`, `MudIconButton`, `MudText`, `MudTooltip`) used inside cells for consistent styling.
- Status label/color must reuse `OrderStatusExtension.GetLabel/GetColor/GetDescription` (`Models/Enums/OrderStatusExtension.cs`), `ColorAttribute` (`Models/Enums/ColorAttribute.cs`), and `OrderStatusEnum` (`Models/Enums/OrderStatusEnum.cs`) — exactly as done in `OrderDashboardCard.razor` and `OrderDetailsDashboardPage.razor` (MudChip with `--status-color` CSS var + `order-status-label`/`order-label-dot` classes).

## Existing conventions discovered (follow these exactly)

- **SP-backed service methods** use `SqlConnection` + `SqlCommand(CommandType.StoredProcedure)` + `SqlDataAdapter` + manual `DataRow` mapping into a `*SP` model, then `_mapper.Map<List<TDto>>(...)` — see `OrderService.GetOrderDetailsDtos` and `GetOrderItemsDTOListById` (`Services/Backend/OrderService.cs`, lines ~87-206). Connection string comes from `DbConfiguration.OnlineStoreDb` (injected `_dbConfiguration`), wrapped in try/catch returning `null` on error with `Console.WriteLine` logging.
- **Model layering**: `Models/StoredProcedureModels/*SP.cs` → `Models/DTOs/*DTO.cs` → `Models/FrontendModels/*Model.cs`, each a plain POCO class.
- **AutoMapper**: all mappings are centralized in `Models/MappingProfiles/OrderMappingProfile.cs` (a single `Profile` subclass registered in DI, presumably via `AddAutoMapper` in `Program.cs`). Existing pattern for enum mapping: `CreateMap<OrderDetailsSP, OrderDetailsDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderStatusEnum)src.StatusId)).ReverseMap();`
- **Controller**: `OrderController` (`Controllers/OrderController.cs`) is a thin wrapper — each action calls one `IOrderService` method and returns `Ok(result)`/`BadRequest()`/`NotFound()`. Admin-only endpoints use `[Authorize(Roles = "Admin")]`.
- **Frontend service**: `OrderFrontendService` (`Services/Frontend/OrderFrontendService.cs`) calls `_apiClient.GetAsync<T>(url)` then `_mapper.Map<TModel>(dto)`, returning an empty list rather than null when no data.
- **Status chip markup** (copy exactly, from `OrderDashboardCard.razor`):
```razor
<MudTooltip Text="@OrderStatusExtension.GetDescription(model.StatusEnum)">
	<MudChip T="string" Class="order-status-label" Size="Size.Small" Variant="Variant.Filled"
			 Style="@($"--status-color: {OrderStatusExtension.GetColor(model.StatusEnum)}")">
		<span class="order-label-dot"></span>
		@OrderStatusExtension.GetLabel(model.StatusEnum)
	</MudChip>
</MudTooltip>
```
with the matching `<style>` block (`.order-status-label`, `.order-label-dot`) copied from `OrderDashboardCard.razor`.
- **Admin nav**: `Components/Layout/NavMenu.razor` has an `<AuthorizeView Roles="Admin"><MudNavGroup Title="Dashboard" Expanded="true">` block where new admin links are added as `MudNavLink`.
- **API client**: all HTTP calls from frontend services go through `IApiClient` (`JWTAuthentication/Services/ApiClient.cs`) — `GetAsync<T>(string requestUri)` handles JWT attachment and 401 redirect automatically. No raw `HttpClient` usage needed.

## New stored procedures (SQL — to be executed manually by the user against the DB; not added as files since no `/Scripts` folder exists in the repo)

```sql
CREATE OR ALTER PROCEDURE dbo.spGetOrdersPaged
	@Offset INT,
	@PageSize INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		o.Id AS OrderId,
		o.OrderNumber,
		o.OrderDate,
		o.StatusId,
		u.Email,
		COUNT(DISTINCT oi.ProductId) AS ProductsNumber,
		SUM(oi.Quantity) AS TotalQuantityItems,
		SUM(oi.Quantity * oi.UnitPrice) AS TotalPrice
	FROM dbo.[Order] o
	LEFT JOIN dbo.OrderItem oi ON oi.OrderId = o.Id
	LEFT JOIN dbo.[User] u ON u.Id = o.UserId
	GROUP BY o.Id, o.OrderNumber, o.OrderDate, o.StatusId, u.Email
	ORDER BY o.OrderDate DESC
	OFFSET @Offset ROWS
	FETCH NEXT @PageSize ROWS ONLY;
END
```

```sql
CREATE OR ALTER PROCEDURE dbo.spGetOrderProductsByOrderId
	@OrderId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		oi.ProductId,
		p.Name AS ProductName,
		oi.Quantity,
		oi.UnitPrice,
		(oi.Quantity * oi.UnitPrice) AS TotalPrice
	FROM dbo.OrderItem oi
	INNER JOIN dbo.Product p ON p.Id = oi.ProductId
	WHERE oi.OrderId = @OrderId;
END
```

(Exact table/column names — `[Order]`, `OrderItem`, `User`, `Product`, `Name` — must be verified by the user against the actual DB schema/`OnlineStoreContext` before running; adjust if column names differ, e.g. product name column.)

## New files

**Stored-procedure models** (`Models/StoredProcedureModels/`):
- `OrderSummarySP.cs` — `OrderId (int), OrderNumber (string), OrderDate (DateTime), StatusId (int), Email (string), ProductsNumber (int), TotalQuantityItems (int), TotalPrice (decimal)`
- `OrderProductSP.cs` — `ProductId (int), ProductName (string), Quantity (int), UnitPrice (decimal), TotalPrice (decimal)`

**DTOs** (`Models/DTOs/`):
- `OrderSummaryDTO.cs` — same fields as `OrderSummarySP` plus `StatusEnum (OrderStatusEnum)` (mapped from `StatusId`), no raw `StatusId` exposed to frontend necessarily (keep `StatusId` too if convenient, or map directly to enum only).
- `OrderProductDTO.cs` — mirrors `OrderProductSP` exactly.

**Frontend models** (`Models/FrontendModels/`):
- `OrderSummaryModel.cs` — `OrderId, OrderNumber, OrderDate, StatusEnum, Email, ProductsNumber, TotalQuantityItems, TotalPrice`.
- `OrderProductModel.cs` — mirrors `OrderProductDTO` exactly.

**Mapping profile** — extend `Models/MappingProfiles/OrderMappingProfile.cs`:
```csharp
CreateMap<OrderSummarySP, OrderSummaryDTO>()
	.ForMember(dest => dest.StatusEnum, opt => opt.MapFrom(src => (OrderStatusEnum)src.StatusId))
	.ReverseMap();

CreateMap<OrderSummaryDTO, OrderSummaryModel>().ReverseMap();

CreateMap<OrderProductSP, OrderProductDTO>().ReverseMap();

CreateMap<OrderProductDTO, OrderProductModel>().ReverseMap();
```

**Backend service** — extend `Services/Backend/Contracts/IOrderService.cs` and `Services/Backend/OrderService.cs`:
- `Task<List<OrderSummaryDTO>?> GetOrdersPaged(int offset, int pageSize)` — same connection/adapter/try-catch pattern as `GetOrderDetailsDtos`, calling `spGetOrdersPaged` with `@Offset`, `@PageSize` params.
- `Task<List<OrderProductDTO>?> GetOrderProductsByOrderId(int orderId)` — same pattern as `GetOrderItemsDTOListById`, calling `spGetOrderProductsByOrderId` with `@OrderId`.

**Controller** — extend `Controllers/OrderController.cs`:
- `GET api/order/get/summary-paged` with query params `offset`/`pageSize`, `[Authorize(Roles = "Admin")]`, calls `GetOrdersPaged`, returns `Ok`/`BadRequest`.
- `GET api/order/get/products/{orderId:int}`, `[Authorize(Roles = "Admin")]`, calls `GetOrderProductsByOrderId`, returns `Ok`/`BadRequest`.

**Frontend service** — extend `Services/Frontend/Contracts/IOrderFrontendService.cs` and `Services/Frontend/OrderFrontendService.cs`:
- `Task<List<OrderSummaryModel>> GetOrdersPaged(int offset, int pageSize)` — calls `_apiClient.GetAsync<List<OrderSummaryDTO>>($"api/order/get/summary-paged?offset={offset}&pageSize={pageSize}")`, maps, returns empty list on null.
- `Task<List<OrderProductModel>> GetOrderProductsByOrderId(int orderId)` — calls `_apiClient.GetAsync<List<OrderProductDTO>>($"api/order/get/products/{orderId}")`, maps, returns empty list on null.

**New page** `Components/Pages/Admin/AdminOrdersPage.razor` + `AdminOrdersPage.razor.cs`:
- Route `@page "/admin/orders"`, guarded with `<AuthorizeView Roles="Admin">` (and ideally `[Authorize]`-equivalent check, consistent with other admin pages).
- Injects `IOrderFrontendService`.
- Renders a plain `<table class="admin-orders-table">` with a `<thead>` row matching the exact column order: triangle (blank `<th>`), Status, Order Id, Order Number, Order Date, Customer Email, Products Number, Total Quantity Items, Total Price.
- `<tbody>` uses `<Virtualize T="OrderSummaryModel" ItemsProvider="LoadOrdersAsync" Context="order">`, rendering for each item: `<AdminOrderRow Order="order" IsExpanded="..." OnToggleExpand="..." />` followed by a conditional expansion `<tr>` with `<td colspan="9">` containing `<AdminOrderProductsTable Items="..." IsLoading="..." />` when expanded.
- Code-behind:
  - `private async ValueTask<ItemsProviderResult<OrderSummaryModel>> LoadOrdersAsync(ItemsProviderRequest request)` — calls `GetOrdersPaged(request.StartIndex, request.Count)`, returns `new ItemsProviderResult<OrderSummaryModel>(items, totalCountIfKnownOrItems.Count)` (if total count isn't returned by the SP, use a sufficiently large placeholder or track "has more" via page size comparison — document this as an open item).
  - `HashSet<int> _expandedOrderIds` to track expansion state.
  - `Dictionary<int, List<OrderProductModel>> _productsCache` to cache fetched products per order (fetch-once).
  - `async Task ToggleExpand(int orderId)` — toggles membership in `_expandedOrderIds`; if newly expanded and not yet cached, sets a per-order loading flag, calls `GetOrderProductsByOrderId(orderId)`, stores in cache, clears loading flag, `StateHasChanged()`.

**Reusable component** `Components/ReusableComponnets/AdminOrderRow.razor`:
- `[Parameter] OrderSummaryModel Order`, `[Parameter] bool IsExpanded`, `[Parameter] EventCallback OnToggleExpand`.
- Renders `<td>` cells in order: triangle `MudIconButton` (`Icons.Material.Filled.PlayArrow`, CSS class toggling `rotate(90deg)` via `IsExpanded`, `OnClick="OnToggleExpand"`), status `MudChip` (pattern above, using `Order.StatusEnum`), `Order.OrderId`, `Order.OrderNumber`, `Order.OrderDate.ToString("dd/MM/yyyy")`, `Order.Email`, `Order.ProductsNumber`, `Order.TotalQuantityItems`, `Order.TotalPrice`.
- Includes a `<style>` block for the triangle rotation transition and copies the `.order-status-label`/`.order-label-dot` CSS from `OrderDashboardCard.razor`.

**Reusable component** `Components/ReusableComponnets/AdminOrderProductsTable.razor`:
- `[Parameter] List<OrderProductModel>? Items`, `[Parameter] bool IsLoading`.
- Shows a `MudProgressCircular` while `IsLoading`; otherwise renders a `MudTable<OrderProductModel>` (not virtualized — nested lists are small) with headers in order: Product ID, Product Name, Quantity, Unit price, Total price, bound to `ProductId, ProductName, Quantity, UnitPrice, TotalPrice`.

**Nav** — `Components/Layout/NavMenu.razor`: add `<MudNavLink Href="/admin/orders">Orders Table</MudNavLink>` inside the existing `<AuthorizeView Roles="Admin"><MudNavGroup Title="Dashboard">` block (distinct label from the existing `/orders-dashboard` link).

## Open items / risks
- Exact DB table/column names in the stored procedure SQL (`[Order]`, `OrderItem`, `User`, `Product`, `Name`) must be verified against the real schema (`DBModels/Order.cs`, `OrderItem.cs`, `Product.cs`, `User.cs`) before running — adjust SQL if names differ.
- `Virtualize`'s `ItemsProviderResult` needs a total count; since `spGetOrdersPaged` only returns a page, the implementation should treat `totalItemCount` as "unknown/large" (e.g., request.StartIndex + items.Count + (items.Count == request.Count ? 1 : 0) heuristic) — verify this matches Blazor's `Virtualize` expectations, or consider adding a `COUNT(*) OVER()` column to the SP result to get an exact total in one round trip (recommended refinement).
- Confirm `AutoMapper` profile registration (likely in `Program.cs` via `services.AddAutoMapper(typeof(OrderMappingProfile))` or assembly scan) automatically picks up the extended `OrderMappingProfile` — no separate registration needed since it's the same class.

## Steps
1. Create `Models/StoredProcedureModels/OrderSummarySP.cs` and `Models/StoredProcedureModels/OrderProductSP.cs` with the fields listed above.
2. Create `Models/DTOs/OrderSummaryDTO.cs` and `Models/DTOs/OrderProductDTO.cs` with the fields listed above.
3. Create `Models/FrontendModels/OrderSummaryModel.cs` and `Models/FrontendModels/OrderProductModel.cs` with the fields listed above.
4. Extend `Models/MappingProfiles/OrderMappingProfile.cs` with the four new `CreateMap` entries (SP→DTO with StatusId→StatusEnum conversion, DTO→Model, and the product pair), following the existing `ForMember`/`ReverseMap` style already in the file.
5. Extend `Services/Backend/Contracts/IOrderService.cs` with `GetOrdersPaged(int offset, int pageSize)` and `GetOrderProductsByOrderId(int orderId)` signatures.
6. Implement both methods in `Services/Backend/OrderService.cs`, copying the `SqlConnection`/`SqlCommand`/`SqlDataAdapter`/`DataRow`-mapping/try-catch pattern from `GetOrderDetailsDtos`/`GetOrderItemsDTOListById`, calling `spGetOrdersPaged` and `spGetOrderProductsByOrderId` respectively.
7. Extend `Services/Frontend/Contracts/IOrderFrontendService.cs` with `GetOrdersPaged(int offset, int pageSize)` and `GetOrderProductsByOrderId(int orderId)`.
8. Implement both methods in `Services/Frontend/OrderFrontendService.cs`, calling `_apiClient.GetAsync<T>` on the new endpoint URLs and mapping via `_mapper`, returning empty lists on null.
9. Extend `Controllers/OrderController.cs` with `GET api/order/get/summary-paged` (query params `offset`, `pageSize`) and `GET api/order/get/products/{orderId:int}`, both `[Authorize(Roles = "Admin")]`, following the thin-wrapper pattern of existing actions.
10. Create `Components/ReusableComponnets/AdminOrderRow.razor` rendering the main row's `<td>` cells in the exact required column order, including the triangle toggle button and the status chip (reusing `OrderStatusExtension`/`ColorAttribute`/`OrderStatusEnum`), with CSS for triangle rotation and the status-chip styles.
11. Create `Components/ReusableComponnets/AdminOrderProductsTable.razor` rendering the nested `MudTable<OrderProductModel>` with columns Product ID | Product Name | Quantity | Unit price | Total price, plus a loading indicator state.
12. Create `Components/Pages/Admin/AdminOrdersPage.razor` with the `<Virtualize>`-based plain `<table>` layout, header row in the exact column order, admin-only guard, and wiring to `AdminOrderRow`/`AdminOrderProductsTable`.
13. Create `Components/Pages/Admin/AdminOrdersPage.razor.cs` implementing `LoadOrdersAsync` (Virtualize `ItemsProvider`), expansion-state tracking (`_expandedOrderIds`), per-order lazy-load-once caching (`_productsCache`), and `ToggleExpand`.
14. Add a `MudNavLink` to `/admin/orders` inside the Admin `MudNavGroup` in `Components/Layout/NavMenu.razor`.
15. Provide the two `CREATE OR ALTER PROCEDURE` SQL scripts to the user (as documented above) to run manually against the database, after they confirm/adjust table and column names against the actual schema.
16. Build and manually verify: infinite scroll loads additional order pages as the admin scrolls, triangle rotates and expands/collapses per row, product sub-table lazy-loads exactly once per order and displays correct columns, and the page is restricted to Admin users only.
