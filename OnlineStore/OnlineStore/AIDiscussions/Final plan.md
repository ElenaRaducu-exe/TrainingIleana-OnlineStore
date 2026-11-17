# 🎯 Admin Orders Page — MudDataGrid Server-Side Pagination + Expandable Product Rows (Reusable OrderProductsList Component)

Full implementation plan based on Draft 3 (`OnlineStore/OnlineStore/AIDiscussions/Draft 3 - GitHub Copilot - Planning.md`), merged with a new reusable, presentation-only `OrderProductsList.razor` component used inside `ChildRowContent`.

**MODIFICATION APPLIED**: Product list loading for an expanded order is no longer lazy/cached. On row expansion, the products for that order are fetched fresh every time via `GetOrderProductsByOrderId(orderId)` — no `_productsCache` dictionary, no `_loadingStates`/`HashSet` tracking, no "cache-check-then-fetch" pattern. Simpler flow: expand → call service → show loading spinner while awaiting → render results. Every expand of the same row re-fetches instead of reusing a cached result.

## Context
Standalone admin feature at a new route: a `MudDataGrid` listing all orders with server-side pagination, where each row expands (via MudDataGrid's built-in hierarchy/triangle feature) to load and show that order's products inside a dedicated reusable component.

Per explicit user decisions:
- New, slim DTOs/Models/SP-models are used instead of reusing `OrderDetailsDTO`/`OrderItemDetailsDTO` (those carry unneeded fields like `Username`, `FullName`, `ProductDescription`, `BrandName`, `CategoryName`, `ModifiedAt`, `AddressId`, `UserId`).
- Two new stored procedures are required (SQL below, run manually by the user in SSMS — no `/Scripts` folder exists in the repo, so no `.sql` file is added to source control).
- No new services/controllers — reuse/extend the **existing** `IOrderService`/`OrderService`, `IOrderFrontendService`/`OrderFrontendService`, and `OrderController`, keeping the same backend-service+contract / frontend-service+contract / controller structure already used in the project.
- Pagination (not infinite scroll) via `MudDataGrid`'s `ServerData` callback.
- Expand/collapse triangle uses `MudDataGrid`'s built-in `HierarchyColumn`/`ChildRowContent` feature (native rotating expand icon) — no custom icon/CSS needed.
- Order model carries `OrderId` but it is **not** shown as a grid column. Product model carries `ProductId` but it is **not** shown as a column.
- Products are fetched from `OrderItem` (joined to `Product`) each time a row is expanded — never preloaded with the orders page, and **no longer cached** across expand/collapse cycles (per latest requirement).
- Status must reuse `OrderStatusExtension.GetLabel/GetColor/GetDescription` (`Models/Enums/OrderStatusExtension.cs`), `ColorAttribute` (`Models/Enums/ColorAttribute.cs`), `OrderStatusEnum` (`Models/Enums/OrderStatusEnum.cs`) — same chip markup/CSS as `OrderDashboardCard.razor` (`MudChip` + `--status-color` CSS var + `order-status-label`/`order-label-dot` classes + `MudTooltip` with `GetDescription`).
- The products list is rendered via a reusable component `Components/ReusableComponnets/OrderProductsList.razor` (+ `.razor.cs`). It is strictly presentation-only: parameters-driven (`Items`, `IsLoading`), no injected services, no API calls, no action buttons, no role checks.

## Existing conventions to follow (confirmed via codebase exploration)
- **SP-backed service methods**: `SqlConnection` + `SqlCommand(CommandType.StoredProcedure)` + `SqlDataAdapter` + manual `DataRow`→`*SP` model mapping, then `_mapper.Map<List<TDto>>(...)`, wrapped in try/catch logging via `Console.WriteLine` and returning `null` on failure. See `OrderService.GetOrderDetailsDtos` / `GetOrderItemsDTOListById` (`Services/Backend/OrderService.cs`). Connection string via injected `DbConfiguration _dbConfiguration.OnlineStoreDb`.
- **Model layering**: `Models/StoredProcedureModels/*SP.cs` → `Models/DTOs/*DTO.cs` → `Models/FrontendModels/*Model.cs`, plain POCOs.
- **AutoMapper**: centralized in `Models/MappingProfiles/OrderMappingProfile.cs`. Existing enum-mapping pattern: `CreateMap<OrderDetailsSP, OrderDetailsDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderStatusEnum)src.StatusId)).ReverseMap();` — follow this exactly for `StatusId → StatusEnum`.
- **Controller**: `OrderController` (`Controllers/OrderController.cs`) is a thin wrapper, one action per service method, `[Authorize(Roles = "Admin")]` for admin-only endpoints, returns `Ok(result)` / `BadRequest()` / `NotFound()`.
- **Frontend service**: `OrderFrontendService` (`Services/Frontend/OrderFrontendService.cs`) calls `_apiClient.GetAsync<T>(url)` (JWT-attached, 401-handling built in via `IApiClient`/`ApiClient.cs`) then `_mapper.Map<TModel>(dto)`, returning empty collections instead of null on no-data.
- **Status chip markup** (copy exactly from `OrderDashboardCard.razor`):
```razor
<MudTooltip Text="@OrderStatusExtension.GetDescription(model.StatusEnum)">
	<MudChip T="string" Class="order-status-label" Size="Size.Small" Variant="Variant.Filled"
			 Style="@($"--status-color: {OrderStatusExtension.GetColor(model.StatusEnum)}")">
		<span class="order-label-dot"></span>
		@OrderStatusExtension.GetLabel(model.StatusEnum)
	</MudChip>
</MudTooltip>
```
plus the matching `.order-status-label` / `.order-label-dot` `<style>` block copied from the same file.
- **Admin nav**: `Components/Layout/NavMenu.razor` has `<AuthorizeView Roles="Admin"><MudNavGroup Title="Dashboard" Expanded="true">` — add the new link here as a `MudNavLink`.
- **API client**: all HTTP calls go through `IApiClient.GetAsync<T>(string requestUri)` (`JWTAuthentication/Services/ApiClient.cs`) — handles JWT + 401 redirect automatically.

## Stored procedures (SQL — user runs manually in SSMS; verify table/column names against actual schema — `DBModels/Order.cs`, `OrderItem.cs`, `Product.cs`, `User.cs` — before executing)

```sql
CREATE OR ALTER PROCEDURE dbo.spGetOrdersPaged
	@PageNumber INT,
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
		SUM(oi.Quantity * oi.UnitPrice) AS TotalPrice,
		COUNT(*) OVER() AS TotalCount
	FROM dbo.[Order] o
	LEFT JOIN dbo.OrderItem oi ON oi.OrderId = o.Id
	LEFT JOIN dbo.[User] u ON u.Id = o.UserId
	GROUP BY o.Id, o.OrderNumber, o.OrderDate, o.StatusId, u.Email
	ORDER BY o.OrderDate DESC
	OFFSET (@PageNumber * @PageSize) ROWS
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

(`COUNT(*) OVER()` gives the total row count across all pages in the same result set, needed for `MudDataGrid`'s `GridData<T>.TotalItems`.)

## New files

**Stored-procedure models** (`Models/StoredProcedureModels/`):
- `OrderSummarySP.cs` — `OrderId (int), OrderNumber (string), OrderDate (DateTime), StatusId (int), Email (string), ProductsNumber (int), TotalQuantityItems (int), TotalPrice (decimal), TotalCount (int)`
- `OrderProductSP.cs` — `ProductId (int), ProductName (string), Quantity (int), UnitPrice (decimal), TotalPrice (decimal)`

**DTOs** (`Models/DTOs/`):
- `OrderSummaryDTO.cs` — `OrderId, OrderNumber, OrderDate, StatusEnum (OrderStatusEnum), Email, ProductsNumber, TotalQuantityItems, TotalPrice` (no `TotalCount` here — kept only at the SP/paged-result level).
- `OrderProductDTO.cs` — mirrors `OrderProductSP` exactly (`ProductId, ProductName, Quantity, UnitPrice, TotalPrice`).
- `OrderSummaryPagedResultDTO.cs` (small wrapper) — `List<OrderSummaryDTO> Items, int TotalCount` — used as the return/response shape for the paged backend method and API endpoint.

**Frontend models** (`Models/FrontendModels/`):
- `OrderSummaryModel.cs` — `OrderId, OrderNumber, OrderDate, StatusEnum, Email, ProductsNumber, TotalQuantityItems, TotalPrice`.
- `OrderProductModel.cs` — mirrors `OrderProductDTO` exactly.
- `OrderSummaryPagedResultModel.cs` — `List<OrderSummaryModel> Items, int TotalCount` (mirrors the DTO wrapper, used by the frontend service/page to feed `MudDataGrid`'s `GridData<T>`).

**Mapping profile** — extend `Models/MappingProfiles/OrderMappingProfile.cs`:
```csharp
CreateMap<OrderSummarySP, OrderSummaryDTO>()
	.ForMember(dest => dest.StatusEnum, opt => opt.MapFrom(src => (OrderStatusEnum)src.StatusId))
	.ReverseMap();

CreateMap<OrderSummaryDTO, OrderSummaryModel>().ReverseMap();

CreateMap<OrderProductSP, OrderProductDTO>().ReverseMap();

CreateMap<OrderProductDTO, OrderProductModel>().ReverseMap();

CreateMap<OrderSummaryPagedResultDTO, OrderSummaryPagedResultModel>().ReverseMap();
```

**Backend service** — extend `Services/Backend/Contracts/IOrderService.cs` and `Services/Backend/OrderService.cs`:
- `Task<OrderSummaryPagedResultDTO?> GetOrdersPaged(int pageNumber, int pageSize)` — executes `spGetOrdersPaged` with `@PageNumber`, `@PageSize`; maps rows to `List<OrderSummarySP>`, reads `TotalCount` from the first row (or 0 if no rows), maps to `List<OrderSummaryDTO>` via `_mapper`, returns the wrapper DTO. Same try/catch/logging pattern as existing methods.
- `Task<List<OrderProductDTO>?> GetOrderProductsByOrderId(int orderId)` — executes `spGetOrderProductsByOrderId` with `@OrderId`, maps to `List<OrderProductSP>` then `List<OrderProductDTO>`.

**Controller** — extend `Controllers/OrderController.cs`:
- `GET api/order/get/summary-paged?pageNumber=&pageSize=`, `[Authorize(Roles = "Admin")]`, calls `GetOrdersPaged`, returns `Ok(result)`/`BadRequest()`.
- `GET api/order/get/products/{orderId:int}`, `[Authorize(Roles = "Admin")]`, calls `GetOrderProductsByOrderId`, returns `Ok(result)`/`BadRequest()`.

**Frontend service** — extend `Services/Frontend/Contracts/IOrderFrontendService.cs` and `Services/Frontend/OrderFrontendService.cs`:
- `Task<OrderSummaryPagedResultModel> GetOrdersPaged(int pageNumber, int pageSize)` — calls `_apiClient.GetAsync<OrderSummaryPagedResultDTO>($"api/order/get/summary-paged?pageNumber={pageNumber}&pageSize={pageSize}")`, maps via `_mapper`, returns an empty-wrapper (`Items = new(), TotalCount = 0`) on null.
- `Task<List<OrderProductModel>> GetOrderProductsByOrderId(int orderId)` — calls `_apiClient.GetAsync<List<OrderProductDTO>>($"api/order/get/products/{orderId}")`, maps, returns empty list on null.

## New reusable component — `Components/ReusableComponnets/OrderProductsList.razor` + `.razor.cs`

**Code-behind:**
```csharp
public partial class OrderProductsList : ComponentBase
{
    [Parameter] public List<OrderProductModel> Items { get; set; } = new();
    [Parameter] public bool IsLoading { get; set; }
}
```
No `[Inject]` services, no internal data-fetch logic inside this component — purely parameter-driven, re-renders by default on parameter change.

**Markup — three render states:**
1. **Loading** (`IsLoading == true`): centered `MudProgressCircular Indeterminate="true" Size="Size.Small"` + `MudText` "Loading products…". Nothing else renders.
2. **Empty** (`Items is null || Items.Count == 0`, not loading): `MudAlert Severity="Severity.Info"` (or muted `MudText`) "No products found for this order."
3. **Data** (otherwise): `MudTable<OrderProductModel> Items="@Items" Dense="true" Hover="true" Elevation="0"`:
   - `HeaderContent`: `Product Name`, `Quantity`, `Unit Price`, `Total Price`.
   - `RowTemplate`: `@context.ProductName`, `@context.Quantity`, `@context.UnitPrice.ToString("C")`, `@context.TotalPrice.ToString("C")`.
   - `ProductId` exists on the model but is never rendered. No action column, no `@onclick` handlers, no `AuthorizeView` role checks.

## New page — `Components/Pages/Admin/AdminOrdersPage.razor` + `AdminOrdersPage.razor.cs`
- Route `@page "/admin/orders"`, guarded with `<AuthorizeView Roles="Admin">` (matching other admin-only pages' pattern).
- Injects `IOrderFrontendService`.
- `<MudDataGrid T="OrderSummaryModel" ServerData="@LoadServerData" ...>` with columns, in exact order:
  1. `<HierarchyColumn T="OrderSummaryModel"/>` (the triangle, no header text — built-in expand/collapse behavior).
  2. Status — `<TemplateColumn Title="Status">` rendering the `MudChip` status pattern above.
  3. Order Number — `<PropertyColumn Property="x => x.OrderNumber" Title="Order Number"/>`
  4. Order Date — `<PropertyColumn Property="x => x.OrderDate" Title="Order Date" Format="dd/MM/yyyy"/>`
  5. Customer Email — `<PropertyColumn Property="x => x.Email" Title="Customer Email"/>`
  6. Products Number — `<PropertyColumn Property="x => x.ProductsNumber" Title="Products Number"/>`
  7. Total Quantity Items — `<PropertyColumn Property="x => x.TotalQuantityItems" Title="Total Quantity Items"/>`
  8. Total Price — `<PropertyColumn Property="x => x.TotalPrice" Title="Total Price"/>`
- `<ChildRowContent>` — renders `<OrderProductsList Items="@_currentOrderProducts" IsLoading="@_isLoadingProducts" />`. **No cache dictionary, no "cache-check-then-fetch" pattern.** Instead:
  - On row expansion, trigger a method (e.g. wired through `ChildRowContent`'s render combined with a per-row expanded-state check, or via the simplest reliable mechanism the installed MudDataGrid version exposes) that: sets `_isLoadingProducts = true`, calls `await GetOrderProductsByOrderId(context.Item.OrderId)` to fetch the full products list for that order every time it is expanded, assigns the result to `_currentOrderProducts`, sets `_isLoadingProducts = false`, then `StateHasChanged()`.
  - Each expand of a row (including re-expanding a previously collapsed row) performs a fresh fetch — no stored/cached results are reused between expand/collapse cycles.
  - If multiple rows can be expanded simultaneously and must show independent product lists at once, use per-order local state inside the `ChildRowContent` template context (e.g. a small inline flag/list captured per render) rather than a single shared pair of page-level fields, so each expanded row shows its own fetched products without needing a cache.
- Code-behind:
  - `private async Task<GridData<OrderSummaryModel>> LoadServerData(GridState<OrderSummaryModel> state)` — calls `GetOrdersPaged(state.Page, state.PageSize)`, returns `new GridData<OrderSummaryModel> { Items = result.Items, TotalItems = result.TotalCount }`.
  - No `_productsCache` dictionary, no `_loadingStates`/`HashSet` fields — removed per this update.

## Nav
`Components/Layout/NavMenu.razor`: add `<MudNavLink Href="/admin/orders">Orders Table</MudNavLink>` inside the existing `<AuthorizeView Roles="Admin"><MudNavGroup Title="Dashboard">` block (distinct label from the existing `/orders-dashboard` link).

## Open items / risks
- Must verify the installed MudBlazor package version in the project supports `MudDataGrid` `HierarchyColumn`/`ChildRowContent` (available MudBlazor v6+) and confirm the exact API for detecting "row just expanded" to trigger the product fetch, since caching is no longer used to short-circuit repeated renders — the fetch-trigger mechanism must avoid re-fetching on every unrelated re-render of the grid (e.g. trigger only on the actual expand action/event, not on every `ChildRowContent` render pass).
- Exact DB table/column names (`[Order]`, `OrderItem`, `User`, `Product`, `Name`) must be confirmed against the real schema before running the SQL; adjust if column names differ.
- Confirm `OrderMappingProfile` is auto-discovered by the existing AutoMapper DI registration (likely `services.AddAutoMapper(typeof(OrderMappingProfile))` or assembly scan in `Program.cs`) — no separate registration needed since it's the same profile class being extended.
- Without caching, if a user repeatedly expands/collapses the same row, each expansion triggers a new API call; this is the explicitly requested behavior (no cache-check-then-fetch pattern), so it is intentional, not a defect.

**Last Updated**: 2026-11-13 08:35:55

## 📝 Plan Steps
-  **Create stored-procedure models — `Models/StoredProcedureModels/OrderSummarySP.cs` and `OrderProductSP.cs` with the fields listed above (including `TotalCount` on `OrderSummarySP`).**
-  **Create DTOs — `Models/DTOs/OrderSummaryDTO.cs`, `OrderProductDTO.cs`, and `OrderSummaryPagedResultDTO.cs` (wrapper with `Items`/`TotalCount`).**
-  **Create frontend models — `Models/FrontendModels/OrderSummaryModel.cs`, `OrderProductModel.cs`, and `OrderSummaryPagedResultModel.cs`.**
-  **Extend `Models/MappingProfiles/OrderMappingProfile.cs` with the five new `CreateMap` entries (SP→DTO with `StatusId→StatusEnum` conversion, DTO→Model, and the paged wrapper mapping).**
-  **Add the two stored procedures (`spGetOrdersPaged`, `spGetOrderProductsByOrderId`) via the exact SQL above — confirm actual table/column names against the real DB schema first.**
-  **Extend backend service — add `GetOrdersPaged` and `GetOrderProductsByOrderId` to `IOrderService.cs` and implement in `OrderService.cs` following the existing `SqlDataAdapter`/`DataSet`/try-catch/AutoMapper conventions.**
-  **Extend frontend service — add matching methods to `IOrderFrontendService.cs` and implement in `OrderFrontendService.cs` via `_apiClient.GetAsync` + `_mapper`, with empty-result fallbacks on null.**
-  **Extend controller — add `GET api/order/get/summary-paged` and `GET api/order/get/products/{orderId:int}` endpoints to `OrderController.cs` with `[Authorize(Roles = "Admin")]`.**
-  **Create the new reusable component `Components/ReusableComponnets/OrderProductsList.razor` + `.razor.cs` implementing the three-state (loading/empty/data) design described above.**
-  **Create `Components/Pages/Admin/AdminOrdersPage.razor` + `.razor.cs` — `MudDataGrid` with `ServerData`, `HierarchyColumn`, the exact column list/order, status chip markup/CSS copied from `OrderDashboardCard.razor`, `ChildRowContent` consuming `OrderProductsList`, and a direct fetch-on-expand flow (no cache dictionary, no cache-check-then-fetch pattern) that re-fetches products every time a row is expanded.**
-  **Update navigation — add the `/admin/orders` `MudNavLink` in `NavMenu.razor` under the existing Admin `MudNavGroup`.**
-  **Build and validate — compile the solution, run `get_errors` to check for analyzer/compile issues, and manually verify pagination, triangle expand/collapse animation, product fetch occurring on every expansion (including re-expansion), the three `OrderProductsList` states, column order/names, and status chip styling match all requirements.**

