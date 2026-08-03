# RestaurantApi

一個以 **ASP.NET Core 8** 打造的餐廳後台管理 REST API，涵蓋分類、菜單品項、訂單三大核心資源的 CRUD 與業務邏輯，並實際透過 Docker 部署到 Render 雲端平台，附上可直接操作的 Swagger 文件。

- 🔗 專案原始碼（本機開發設定）：`master` 分支
- 🔗 部署設定（Render 專用設定）：`deployment-render` 分支
- 🔗 線上 API：https://restaurantapi-1-8xwz.onrender.com
- 🔗 線上 Swagger 文件：https://restaurantapi-1-8xwz.onrender.com/swagger
> Render 目前採用免費方案，資料庫將於 2026 年 9 月 1 日關閉會後無法測試線上 API

---

## 專案目的

這個專案用來練習並展示如何在**單一專案**架構下，仍維持清楚的關注點分離來設計一支中小型後台 API：

- 用 DTO 隔離對外契約與資料庫實體，避免 Entity 直接外洩
- 用 EF Core 關聯設定與刪除策略保護資料一致性
- 用同一份程式碼透過分支管理，切換「本機開發（SQLite）」與「雲端正式環境（PostgreSQL + Docker）」兩種執行方式

---

## 功能特色

| 資源 | 說明 |
| --- | --- |
| **Category（分類）** | 菜單分類的建立、查詢、修改、刪除 |
| **MenuItem（菜單品項）** | 品項建立、依分類篩選查詢、上下架狀態、與分類的外鍵驗證 |
| **Order（訂單）** | 下單（含多品項、下單當下單價快照）、依狀態篩選、訂單狀態流轉、刪除 |

### API 端點一覽

**CategoryController** — `api/category`

| Method | Route | 說明 |
| --- | --- | --- |
| GET | `/` | 取得所有分類 |
| GET | `/{id}` | 取得單一分類 |
| POST | `/` | 新增分類 |
| PUT | `/{id}` | 更新分類 |
| DELETE | `/{id}` | 刪除分類 |

**MenuItemsController** — `api/menuitems`

| Method | Route | 說明 |
| --- | --- | --- |
| GET | `/?categoryId={id}` | 取得品項清單，可依分類篩選 |
| GET | `/{id}` | 取得單一品項 |
| POST | `/` | 新增品項（驗證 CategoryId 是否存在） |
| PUT | `/{id}` | 更新品項 |
| DELETE | `/{id}` | 刪除品項 |

**OrderController** — `api/order`

| Method | Route | 說明 |
| --- | --- | --- |
| GET | `/?status={status}` | 取得訂單清單，可依狀態篩選，依建立時間倒序 |
| GET | `/{id}` | 取得單一訂單（含品項明細） |
| POST | `/` | 建立訂單（驗證品項存在、庫存上架狀態、數量合法） |
| PUT | `/{id}/status` | 更新訂單狀態（Pending → Preparing → Completed / Cancelled） |
| DELETE | `/{id}` | 刪除訂單 |

---

## 系統架構 / 專案結構

```
RestaurantApi/
├── Controllers/            # API 進入點，僅負責請求驗證與 DTO 轉換
│   ├── CategoryController.cs
│   ├── MenuItemsController.cs
│   └── OrderController.cs
├── Models/                  # EF Core 資料實體（Entity）
│   ├── Category.cs
│   ├── MenuItem.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── OrderStatus.cs
├── DTOs/                     # 對外契約（Request / Response），與 Entity 分離
│   ├── CategoryDto.cs
│   ├── MenuItemDto.cs
│   └── OrderDto.cs
├── Data/
│   ├── RestaurantDbContext.cs  # DbContext，集中管理關聯與刪除策略
│   └── DbInitializer.cs        # 開發環境自動種子資料
├── Migrations/               # EF Core Code First 版本控制
├── Program.cs                 # 應用程式進入點與服務註冊
├── Dockerfile                 # 僅存在於 deployment-render 分支
└── RestaurantApi.csproj
```

### 資料模型關聯

```
Category (1) ──< MenuItem (N)
Order (1) ──< OrderItem (N) >── MenuItem (1)
```

- `MenuItem → Category`：`DeleteBehavior.Restrict`，避免刪除分類時誤刪品項
- `OrderItem → Order`：`DeleteBehavior.Cascade`，刪除訂單時一併清除明細
- `OrderItem → MenuItem`：`DeleteBehavior.Restrict`，保護品項不被訂單明細牽連刪除

---

## 使用技術

| 類別 | 技術 |
| --- | --- |
| 框架 | .NET 8 / ASP.NET Core Web API |
| ORM | Entity Framework Core 8（Code First + Migrations） |
| 資料庫 | SQLite（本機開發）／PostgreSQL（Render 正式環境，Npgsql） |
| API 文件 | Swashbuckle.AspNetCore（Swagger / OpenAPI） |
| 容器化 | Docker（Multi-stage build） |
| 雲端部署 | Render（Web Service） |

---

## 分支說明：`master` vs `deployment-render`

同一份業務邏輯（Controllers / Models / DTOs）完全共用，差異只集中在「執行環境設定」，展示如何讓一套程式碼適應不同部署目標：

| 項目 | `master`（本機開發） | `deployment-render`（雲端部署） |
| --- | --- | --- |
| 資料庫 | SQLite（`Microsoft.EntityFrameworkCore.Sqlite`） | PostgreSQL（`Npgsql.EntityFrameworkCore.PostgreSQL`） |
| 連線字串 | `Data Source=restaurant.db` | 由 Render 提供的 PostgreSQL 連線字串 |
| Port 綁定 | 預設 Kestrel 設定 | 讀取環境變數 `PORT`，動態綁定 `0.0.0.0` |
| 容器化 | 無 | 提供 `Dockerfile`（多階段建置：SDK 建置 → ASP.NET Runtime 執行） |
| Swagger | 開發環境自動啟用 | 沿用相同的開發環境判斷邏輯（`ASPNETCORE_ENVIRONMENT`），實際啟用與否取決於 Render 後台的環境變數設定 |

---

## 快速開始（本機執行）

```bash
git clone -b master https://github.com/Linoe/RestaurantApi.git
cd RestaurantApi

dotnet restore
dotnet ef database update
dotnet run
```

執行後開啟瀏覽器進入 `https://localhost:{port}/swagger` 即可測試所有 API，開發環境會自動寫入預設分類與菜單種子資料。

---

## 部署重點（Render）

`deployment-render` 分支的 `Dockerfile` 採用多階段建置，先以 SDK image 還原、發佈，再以精簡的 ASP.NET Runtime image 執行，降低最終映像大小；`Program.cs` 會讀取 Render 注入的 `PORT` 環境變數並綁定 `0.0.0.0`，符合 Render Web Service 的連線需求。

---

## 設計重點

- **價格快照**：`OrderItem.UnitPrice` 會記錄下單當下的單價，即使日後菜單改價，也不會影響歷史訂單金額，如：

```csharp
public class OrderItem
{
    public int MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
    public int Quantity { get; set; }

    // 下單當下的單價快照，避免菜單改價後影響歷史訂單金額
    public decimal UnitPrice { get; set; }
}
```

- **請求前置驗證**：建立品項、建立訂單時都會先驗證外鍵是否存在（如 `CategoryId`、`MenuItemId`），並檢查品項是否為上架狀態，避免產生無效資料。
- **延遲查詢組合**：篩選條件（`categoryId`、`status`）皆以 `IQueryable` 疊加 `Where`，SQL 只在最終 `ToListAsync()` 時組合執行，避免不必要的資料庫往返。
- **關注點分離**：Controller 只做請求驗證與 DTO 轉換，資料規則（精度、外鍵、刪除策略）集中於 `RestaurantDbContext.OnModelCreating`，維持單一職責。

---

## 後續設計

目前專案聚焦在展示 CRUD 與資料關聯設計，若朝向正式商用，**安全補強**是最優先的設計方向，其餘資料完整性與維運機制則列為次要階段。

### 安全補強（最高優先）

| 現況缺口 | 風險 | 改善方向 |
| --- | --- | --- |
| `appsettings.Development.json` 明碼寫死 PostgreSQL 密碼 | 密碼隨原始碼進版控，一旦推上公開 repo 即外洩 | 改用 `dotnet user-secrets`（本機）／環境變數或 Render Secret Files（雲端），連線字串不進版控 |
| 所有 Controller 皆無 `[Authorize]` | 任何人都能直接新增、修改、刪除分類／品項／訂單資料 | 導入 JWT Bearer 驗證，區分顧客端與後台管理權限（Role-based Authorization） |
| 無全域例外處理 middleware | 未預期例外會回傳完整 Exception 堆疊，暴露內部程式路徑與框架細節 | 加上 `UseExceptionHandler`／自訂 Middleware，統一回傳精簡錯誤格式，隱藏堆疊資訊 |
| 無 CORS 白名單設定 | 目前預設允許任意來源呼叫 API | 正式環境改為明確指定允許的前端網域 |
| Docker image 內含連線字串等機密資訊的執行環境 | Image 外流即等同洩漏資料庫存取權 | 機密改由 Render Environment Groups／雲端 Secret 管理注入，不寫入 image 或 appsettings |

### 資料完整性與可靠性（次要階段）

- 補上 DataAnnotations／FluentValidation，取代目前分散在 Controller 內的手動 `if` 驗證（例如 `CustomerName` 目前可為空字串）
- `UpdateOrderStatus` 缺乏併發控制（RowVersion／Concurrency Token），多人同時修改同一筆訂單狀態會互相覆蓋
- `GetMenuItems`／`GetOrders` 缺乏分頁機制，資料量成長後會整包回傳造成效能問題

### 維運與品質（次要階段）

- 補上 `/health` 健康檢查端點，供 Render 或未來的負載平衡機制判斷服務狀態
- 建立測試專案（xUnit + WebApplicationFactory），涵蓋 Controller 與業務規則
- 建立 GitHub Actions CI/CD，自動化建置、測試、部署到 Render
- 導入 API 版本控制（如 `Asp.Versioning`），避免未來欄位異動直接破壞既有客戶端

### 架構擴充方向（次要階段）

目前為單一專案架構，Controller 直接操作 `DbContext`，對這個規模的專案是合理選擇；若業務量與需求複雜度成長，可考慮：

- 拆出 Service Layer（或依領域拆分多個專案），取代 Controller 直接呼叫 `DbContext` 的做法，降低耦合、方便單元測試
- 若未來需要多角色權限、金流串接、訂單狀態通知等更複雜的流程，可評估導入 CQRS 或訊息佇列（如 RabbitMQ／Azure Service Bus），避免商業邏輯持續集中在 Controller 內

---

## 授權

本專案為個人作品集展示用途。
