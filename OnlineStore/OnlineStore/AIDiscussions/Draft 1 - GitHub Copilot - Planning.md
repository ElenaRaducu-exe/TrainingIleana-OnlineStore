# Admin Orders Table with Expandable Product Rows

## Context from codebase exploration

Existing infrastructure that must be reused (no need to recreate backend logic):

- **Backend DTOs** (`OnlineStore/Models/DTOs/`):
  - `OrderDetailsDTO` — already has `OrderId, OrderNumber, OrderDate, ProductsNumber, TotalQuantityItems, TotalPrice, Status (OrderStatusEnum), Email, Username, FullName, ModifiedAt` — covers every main-row column needed.
  - `OrderItemDetailsDTO` — already has `ProductId, ProductName, Quantity, UnitPrice, TotalPrice` — covers every nested product-row column needed.
- **Backend service** `OrderService` (`Services/Backend/OrderService.cs`) + contract `IOrderService`:
  - `GetOrderDetailsDtos(int? customerId)` → list of orders (calls `spGetOrders`).
  - `GetOrderItemsDTOListById(int orderId)` → list of order items for one order (calls `spGetOrderItemsDetails`).
- **API controller** `OrderController` (`Controllers/OrderController.cs`):
  - `GET api/order/get/list` (`[Authorize(Roles = "Admin")]`) → all orders.
  - `GET api/order/get/items/{orderId:int}` (`[Authorize(Roles = "Admin")]`) → order items for one order. This is exactly the lazy-load endpoint needed.
- **Frontend models** (`Models/FrontendModels/`): `OrderDetailsModel` and `OrderItemDetailsModel` mirror the DTOs and are already AutoMapper-mapped from them.
- **Frontend service** `OrderFrontendService` + contract `IOrderFrontendService` (`Services/Frontend/`):
  - `GetOrderDetailsModelList(bool isAdminUser)` already calls `api/order/get/list` for admins.
  - There is **no** existing frontend method to call `api/order/get/items/{orderId}` — this must be added.
- **Existing UI patterns to follow**:
  - `OrdersDashboard.razor` (route `/orders-dashboard`) shows how a `MudTable<OrderDetailsModel>` is built, with a `RowTemplate` delegating cell rendering to a reusable component `OrderDashboardCard.razor` (`Components/ReusableComponnets/`).
  - `OrderDashboardCard.razor` shows the exact pattern for rendering a status chip using `OrderStatusExtension.GetColor/GetLabel/GetDescription` (from `Models/Enums/OrderStatusEnum.cs`, which has a `[Color]` attribute per status) — reuse this for the Status column/chip.
  - `NavMenu.razor` has an `Admin`-only nav group (`Components/Layout/NavMenu.razor`) where a link to the new page should be added.
  - MudBlazor is the UI library in use (`MudTable`, `MudTh`, `MudTd`, `MudChip`, `MudTooltip`, `MudIconButton`, `MudIcon`).

Per user's decisions:
1. This is a **new, separate page** (does not replace `/orders-dashboard`).
2. Nested product list uses a **nested `MudTable`**.
3. Order items are **lazy-loaded** per row on first expand, via the existing `api/order/get/items/{orderId}` endpoint (new frontend service method needed since it doesn't exist yet).

## Design for expandable rows

MudBlazor's `MudTable` supports multi-row templates (`RowTemplate` + a second `<RowTemplate>`-style nested row) via placing a second `<tr>`-equivalent manually, OR by using `MudTable`'s built-in child-row expansion pattern: render the main row via `RowTemplate`, and immediately follow it with a conditionally-rendered expansion row (`<MudTr>`/`<tr>` spanning all columns) containing the nested `MudTable` of products, toggled by a `HashSet<int>` of expanded Order IDs tracked in the page's code-behind. This mirrors common MudBlazor "master-detail" table patterns and avoids needing `MudTable`'s complex built-in grouping features.

The triangle icon: use `MudIconButton` with `Icons.Material.Filled.PlayArrow` (or `ArrowRight`), rotated via a CSS class transition (`transform: rotate(90deg)` when expanded) applied conditionally based on whether the order's ID is in the expanded set.

## New/changed files

1. `Models/FrontendModels/OrderItemDetailsModel.cs` — already exists, reuse as-is.
2. `Services/Frontend/Contracts/IOrderFrontendService.cs` — add `Task<List<OrderItemDetailsModel>> GetOrderItemsList(int orderId);`.
3. `Services/Frontend/OrderFrontendService.cs` — implement `GetOrderItemsList` calling `_apiClient.GetAsync<List<OrderItemDetailsDTO>>($"api/order/get/items/{orderId}")` and mapping to `List<OrderItemDetailsModel>` via `_mapper`.
4. New page: `Components/Pages/Admin/AdminOrdersTable.razor` (+ `.razor.cs` code-behind), routed e.g. `@page "/admin/orders"`, restricted with `[Authorize(Roles = "Admin")]` / `<AuthorizeView Roles="Admin">` wrapper consistent with other admin pages.
5. New reusable component: `Components/ReusableComponnets/AdminOrderRow.razor` (+ code-behind if needed) — renders one order's main `<MudTd>` cells (triangle toggle button, status chip, OrderId, OrderNumber, OrderDate, Email, ProductsNumber, TotalQuantityItems, TotalPrice) and raises an event/callback when the triangle is clicked, OR the parent page handles toggle state directly and this component just renders cells (following `OrderDashboardCard` pattern of being a "dumb" cell renderer).
6. New reusable component: `Components/ReusableComponnets/AdminOrderProductsTable.razor` — given a `List<OrderItemDetailsModel>` (or orderId to lazy-load), renders the nested `MudTable` with columns Product ID, Product Name, Quantity, Unit price, Total price.
7. `Components/Layout/NavMenu.razor` — add a `MudNavLink` to the new admin page inside the existing `<AuthorizeView Roles="Admin">` / `MudNavGroup Title="Dashboard"` block.

## Column order (exact, per user spec)

triangle (no header text) | Status | Order Id | Order Number | Order Date | Customer Email | Products Number | Total Quantity Items | Total Price

Nested product table columns: Product ID | Product Name | Quantity | Unit price | Total price

## Steps

1. Add `GetOrderItemsList(int orderId)` to `IOrderFrontendService` (`Services/Frontend/Contracts/IOrderFrontendService.cs`) returning `Task<List<OrderItemDetailsModel>>`.
2. Implement `GetOrderItemsList` in `OrderFrontendService.cs` — call `_apiClient.GetAsync<List<OrderItemDetailsDTO>>($"api/order/get/items/{orderId}")`, map with `_mapper.Map<List<OrderItemDetailsModel>>(...)`, return empty list on null, following the same null-check pattern as `GetOrderDetailsModelList`.
3. Verify the AutoMapper profile already maps `OrderItemDetailsDTO -> OrderItemDetailsModel` (if not present, add the `CreateMap` entry in the mapping profile class used for the other Order DTO/Model pairs).
4. Create `Components/Pages/Admin/AdminOrdersTable.razor` with `@page "/admin/orders"` and an `[Authorize(Roles = "Admin")]`/`AuthorizeView` guard; render a `MudTable<OrderDetailsModel>` with the exact header column order specified above (triangle column has no `MudTh` text), binding `Items="@ordersList"`.
5. Create `Components/Pages/Admin/AdminOrdersTable.razor.cs` code-behind: inject `IOrderFrontendService`; on `OnInitializedAsync` call `GetOrderDetailsModelList(true)` to populate `ordersList`; maintain a `HashSet<int> _expandedOrderIds` and a `Dictionary<int, List<OrderItemDetailsModel>> _orderItemsCache`; implement `ToggleExpand(int orderId)` that adds/removes from `_expandedOrderIds` and, on first expansion only, calls `GetOrderItemsList(orderId)` and stores result in the cache (lazy-load, cached thereafter so re-collapsing/expanding doesn't refetch).
6. Create `Components/ReusableComponnets/AdminOrderRow.razor` to render the main row's cells in the required order: a `MudIconButton` (rotating triangle icon bound to expansion state) with no header label, `MudChip` status (reusing `OrderStatusExtension.GetColor/GetLabel/GetDescription` + `MudTooltip`, copying the CSS pattern from `OrderDashboardCard.razor`), then `OrderId`, `OrderNumber`, `OrderDate` (formatted `dd/MM/yyyy`), `Email`, `ProductsNumber`, `TotalQuantityItems`, `TotalPrice`. Expose a parameter `bool IsExpanded` and `EventCallback OnToggleExpand` for the triangle button.
7. In `AdminOrdersTable.razor`'s `RowTemplate`, render `<AdminOrderRow OrderModel="@context" IsExpanded="@_expandedOrderIds.Contains(context.OrderId)" OnToggleExpand="@(() => ToggleExpand(context.OrderId))" />`, and immediately after, add a conditional expansion row (a `<tr>`/`MudTr` with a single `<td colspan="9">`) that renders `<AdminOrderProductsTable Items="@GetItemsForOrder(context.OrderId)" />` only when `_expandedOrderIds.Contains(context.OrderId)` is true (handle the loading state, e.g. show a `MudProgressCircular` while items are being fetched for that order the first time).
8. Create `Components/ReusableComponnets/AdminOrderProductsTable.razor` accepting `[Parameter] List<OrderItemDetailsModel> Items`; render a nested `MudTable<OrderItemDetailsModel>` with headers `Product ID | Product Name | Quantity | Unit price | Total price` in that exact order, mapped to `ProductId, ProductName, Quantity, UnitPrice, TotalPrice`.
9. Add CSS for the triangle rotation: a class (e.g. `.triangle-icon { transition: transform 0.2s ease; }` and `.triangle-icon.expanded { transform: rotate(90deg); }`) applied to the `MudIconButton`/`MudIcon` in `AdminOrderRow.razor`, following the `<style>` block pattern already used inline in `OrderDashboardCard.razor`.
10. Add a `MudNavLink` entry to `Components/Layout/NavMenu.razor` inside the existing `<AuthorizeView Roles="Admin"> <MudNavGroup Title="Dashboard">` block, pointing to `/admin/orders` (e.g. labeled "Orders Table" or similar, to be distinguished from the existing "Orders" link to `/orders-dashboard`).
11. Build and manually verify: table renders with correct column order, triangle rotates and expands/collapses, product sub-table displays with correct columns, lazy-load fetch hits `api/order/get/items/{orderId}` only once per order, and role restriction to Admin works correctly.

## Risks / open items
- Confirm exact MudBlazor version supports manually injecting extra `<tr>` rows after `RowTemplate` inside `MudTable` (may require using `@foreach` over `ordersList` manually instead of `MudTable`'s built-in `Items`/`RowTemplate`, or using `MudTable`'s `ChildRowContent`/nested table slot if available in the installed version — verify against installed MudBlazor version in the project before finalizing markup approach).
- Verify AutoMapper profile indeed already maps `OrderItemDetailsDTO` → `OrderItemDetailsModel` (used by `OrderDetailsPageModel.OrderItems` elsewhere) before assuming step 3 is a no-op.
