# Admin Orders Page — MudDataGrid with Server-Side Pagination + Expandable Product Rows

## Context

Standalone admin feature at a new route: a `MudDataGrid` listing all orders with server-side pagination, where each row can be expanded (via MudDataGrid's built-in hierarchy/triangle feature) to lazy-load and show that order's products.

Per explicit user decisions:
- New, slim DTOs/Models/SP-models are used instead of reusing `OrderDetailsDTO`/`OrderItemDetailsDTO` (those carry unneeded fields like `Username`, `FullName`, `ProductDescription`, `BrandName`, `CategoryName`, `ModifiedAt`, `AddressId`, `UserId`).
- Two new stored procedures are required (SQL provided below, to be run manually by the user in SSMS — no `/Scripts` folder exists in the repo, so no `.sql` file is added to source control).
- No new services/controllers — reuse/extend the **existing** `IOrderService`/`OrderService`, `IOrderFrontendService`/`OrderFrontendService`, and `OrderController`, keeping the same backend-service+contract / frontend-service+contract / controller structure already used in the project.
- Pagination (not infinite scroll) via `MudDataGrid`'s `ServerData` callback.
- Expand/collapse triangle uses `MudDataGrid`'s built-in `HierarchyColumn`/`ChildRowContent` feature (native rotating expand icon) — no custom icon/CSS needed.
- Order model carries `OrderId` but it is **not** shown as a grid column. Product model carries `ProductId` but it is **not** shown as a nested-grid column.
- Products are lazy-loaded from `OrderItem` (joined to `Product`) only when a row is first expanded — never preloaded with the orders page.
- Status must reuse `OrderStatusExtension.GetLabel/GetColor/GetDescription` (`Models/Enums/OrderStatusExtension.cs`), `ColorAttribute` (`Models/Enums/ColorAttribute.cs`), `OrderStatusEnum` (`Models/Enums/OrderStatusEnum.cs`) — same chip markup/CSS as `OrderDashboardCard.razor`/`OrderDetailsDashboardPage.razor` (`MudChip` + `--status-color` CSS var + `order-status-label`/`order-label-dot` classes + `MudTooltip` with `GetDescription`).

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

**New page** `Components/Pages/Admin/AdminOrdersPage.razor` + `AdminOrdersPage.razor.cs`:
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
- `<ChildRowContent>` — renders a nested `MudTable<OrderProductModel>` (or nested `MudDataGrid`, simplest as `MudTable` since no sub-paging needed) bound to that order's cached products list, columns in order: Product Name, Quantity, Unit price, Total price (no Product Id column, though `OrderProductModel.ProductId` exists).
- Code-behind:
  - `private async Task<GridData<OrderSummaryModel>> LoadServerData(GridState<OrderSummaryModel> state)` — calls `GetOrdersPaged(state.Page, state.PageSize)`, returns `new GridData<OrderSummaryModel> { Items = result.Items, TotalItems = result.TotalCount }`.
  - `Dictionary<int, List<OrderProductModel>> _productsCache` — keyed by `OrderId`, populated lazily.
  - `Dictionary<int, bool> _loadingStates` (or a `HashSet<int>`) to show a loading indicator while products are being fetched for a given order on first expansion.
  - A method triggered on row expansion (via `ChildRowContent`'s context, checking/populating the cache on first access, e.g. an `async Task EnsureProductsLoaded(int orderId)` called from `OnInitialized`-equivalent inside `ChildRowContent`, or via `MudDataGrid`'s row-expanded event if exposed by the installed MudBlazor version) that calls `GetOrderProductsByOrderId(orderId)` exactly once per order and caches the result.

**Nav** — `Components/Layout/NavMenu.razor`: add `<MudNavLink Href="/admin/orders">Orders Table</MudNavLink>` inside the existing `<AuthorizeView Roles="Admin"><MudNavGroup Title="Dashboard">` block (distinct label from the existing `/orders-dashboard` link).

## Open items / risks
- Must verify the installed MudBlazor package version in the project supports `MudDataGrid` `HierarchyColumn`/`ChildRowContent` (available MudBlazor v6+) and confirm the exact API for detecting "row just expanded" to trigger the lazy product load (some versions expose this via the `ChildRowContent`'s render callback only, without an explicit "expanded" event — in that case, lazy-load check must happen inside the child content itself using a cache-check-then-fetch pattern, safe to call on every render since it short-circuits via the cache dictionary).
- Exact DB table/column names (`[Order]`, `OrderItem`, `User`, `Product`, `Name`) must be confirmed by the user against the real schema before running the SQL; adjust if column names differ.
- Confirm `OrderMappingProfile` is auto-discovered by the existing AutoMapper DI registration (likely `services.AddAutoMapper(typeof(OrderMappingProfile))` or assembly scan in `Program.cs`) — no separate registration needed since it's the same profile class being extended.

## Steps
1. Create `Models/StoredProcedureModels/OrderSummarySP.cs` and `Models/StoredProcedureModels/OrderProductSP.cs` with the fields listed above (including `TotalCount` on `OrderSummarySP`).
2. Create `Models/DTOs/OrderSummaryDTO.cs`, `Models/DTOs/OrderProductDTO.cs`, and `Models/DTOs/OrderSummaryPagedResultDTO.cs` (wrapper with `Items`/`TotalCount`).
3. Create `Models/FrontendModels/OrderSummaryModel.cs`, `Models/FrontendModels/OrderProductModel.cs`, and `Models/FrontendModels/OrderSummaryPagedResultModel.cs`.
4. Extend `Models/MappingProfiles/OrderMappingProfile.cs` with the five new `CreateMap` entries (SP→DTO with `StatusId`→`StatusEnum`, DTO→Model pairs, and the paged-result wrapper), following the existing `ForMember`/`ReverseMap` style.
5. Extend `Services/Backend/Contracts/IOrderService.cs` with `GetOrdersPaged(int pageNumber, int pageSize)` and `GetOrderProductsByOrderId(int orderId)` signatures.
6. Implement both methods in `Services/Backend/OrderService.cs`, copying the `SqlConnection`/`SqlCommand`/`SqlDataAdapter`/`DataRow`-mapping/try-catch pattern from `GetOrderDetailsDtos`/`GetOrderItemsDTOListById`, calling `spGetOrdersPaged` (reading `TotalCount` from the first returned row) and `spGetOrderProductsByOrderId` respectively.
7. Extend `Services/Frontend/Contracts/IOrderFrontendService.cs` with `GetOrdersPaged(int pageNumber, int pageSize)` returning `Task<OrderSummaryPagedResultModel>` and `GetOrderProductsByOrderId(int orderId)` returning `Task<List<OrderProductModel>>`.
8. Implement both methods in `Services/Frontend/OrderFrontendService.cs`, calling `_apiClient.GetAsync<T>` on the new endpoint URLs and mapping via `_mapper`, returning safe empty defaults on null.
9. Extend `Controllers/OrderController.cs` with `GET api/order/get/summary-paged` (query params `pageNumber`, `pageSize`) and `GET api/order/get/products/{orderId:int}`, both `[Authorize(Roles = "Admin")]`, following the thin-wrapper pattern of existing actions.
10. Create `Components/Pages/Admin/AdminOrdersPage.razor` with the `MudDataGrid` markup: `HierarchyColumn`, status `TemplateColumn` (reusing the `MudChip`/`OrderStatusExtension` pattern and its CSS), and the remaining `PropertyColumn`s in the exact required order; add `ChildRowContent` rendering the nested products `MudTable` with columns Product Name, Quantity, Unit price, Total price.
11. Create `Components/Pages/Admin/AdminOrdersPage.razor.cs` implementing `LoadServerData` (MudDataGrid `ServerData` callback using `GetOrdersPaged`), the per-order products cache/loading-state tracking, and the lazy-load-on-first-expand logic used by `ChildRowContent`.
12. Add a `MudNavLink` to `/admin/orders` inside the Admin `MudNavGroup` in `Components/Layout/NavMenu.razor`.
13. Provide the two `CREATE OR ALTER PROCEDURE` SQL scripts to the user (as documented above) to run manually against the database, after confirming/adjusting table and column names against the actual schema.
14. Build and manually verify: grid paginates correctly using `ServerData`, triangle (hierarchy icon) expands/collapses and rotates natively, product sub-table lazy-loads exactly once per order with correct columns, and the page/endpoints are restricted to Admin users only.
