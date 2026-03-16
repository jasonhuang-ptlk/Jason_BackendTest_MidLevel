# 後端工程師技術測試

## 技術要求

- .NET Core 8 Web API
- SQL Server 2019+：資料庫 `Myoffice_ACPD`
- 不限制 Entity Framework Core 或任何 ORM
- Visual Studio 2022（需能執行 Swagger）
- Git & GitHub

**參考資料：**（位於 `TSQLScript` 目錄）
- `TSQL_Myoffice_ACPD.sql`：資料表結構定義
- `NewSID_自訂一組固定欄位的代碼.sql`：主鍵產生範例（選用參考）
- `TSQL_Myoffice_ExcuteionLog.sql`：執行日誌資料表（選用參考）
- `usp_AddLog 記錄執行錯誤.sql`：日誌記錄 SP（選用參考）

---

## 考試時限
- 請在收到考題後的 2 小時內完成並提交
- 時間從收到考題通知開始計算
- 若有特殊情況無法於時限內完成，請主動來信說明原因

---

## 專案要求

### Web API
- 實作完整 CRUD 功能，與資料庫互動（可使用 ORM、Dapper、ADO.NET 或 Stored Procedure）
- 所有資料傳遞使用 JSON 格式（輸入/輸出）
- Swagger 需包含測試用 JSON 資料並可正常呼叫
- 按 F5 即可啟動並顯示 Swagger 介面

**1. 正確使用 HTTP Method**
- `GET`：查詢資料（不可使用 GET 進行資料異動）
- `POST`：新增資料
- `PUT / PATCH`：更新資料
- `DELETE`：刪除資料

**2. 資源導向 URL 設計**
- URL 必須為資源導向（Resource-based）
- 不可出現 `/GetData`、`/DeleteData` 這類動作型命名

```
GET    /api/myofficeacpd          # 查詢所有資料
GET    /api/myofficeacpd/{id}     # 查詢單筆資料
POST   /api/myofficeacpd          # 新增資料
PUT    /api/myofficeacpd/{id}     # 更新資料
DELETE /api/myofficeacpd/{id}     # 刪除資料
```

**3. 必須回傳正確的 HTTP Status Code**
- `200 OK`：請求成功
- `201 Created`：資源成功建立
- `204 No Content`：請求成功但無回傳內容（通常用於 DELETE）
- `400 Bad Request`：請求參數有誤
- `404 Not Found`：資源不存在
- `500 Internal Server Error`：伺服器內部錯誤

**4. Swagger 測試**
- Swagger 必須可正常測試所有 RESTful Endpoint
- 每個 API 附有測試用 JSON 資料
- 可直接在 Swagger UI 進行完整的 CRUD 操作測試

### SQL Server
- 針對 `Myoffice_ACPD` 資料表實作 CRUD 功能：
  - 新增資料：主鍵產生方式自行設計（可參考 `TSQLScript` 目錄內的 `NEWSID` 範例）
  - 查詢資料：支援單筆與多筆查詢
  - 更新資料：根據主鍵更新資料
  - 刪除資料：根據主鍵刪除資料
- 產出資料庫備份檔 `.bak`
- 資料庫備份檔須可正常還原並包含完整測試資料
- 如有使用 SQL Script，請一併推送至 GitHub

### Git 版本管理
- Repository 命名：`YourName_BackendTest_MidLevel`
- `main` 分支：存放最終版本
- `develop` 分支：開發主分支
- 建立至少一個 feature 分支進行開發
- Commit 訊息需清楚描述修改內容

---

## 繳交清單

- [ ] GitHub Repository（包含分支管理記錄）
- [ ] .NET Core 8 Web API 專案原始碼
- [ ] Swagger 可正常執行 CRUD，每個 API 附有測試 JSON
- [ ] SQL Server 資料庫備份檔 `.bak`（包含測試資料）
- [ ] README 說明專案架構與執行步驟
- [ ] 提供 GitHub Repository URL