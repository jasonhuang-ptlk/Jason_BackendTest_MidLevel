# Jason_BackendTest_MidLevel

## 專案說明

.NET Core 8 Web API — 針對 `Myoffice_ACPD` 資料表實作完整 RESTful CRUD，
並透過 `usp_AddLog` 預存程序記錄每次 API 執行的完整流程。

---

## 環境需求

| 項目 | 版本 |
|------|------|
| .NET SDK | 8.0+ |
| SQL Server | 2019+ |
| Visual Studio | 2022 |

---

## 資料庫設定

1. 開啟 SSMS，連線至 **JASONHUANG**（Windows 驗證）
2. 建立資料庫 `Myoffice_ACPD`
3. 依序執行 `TSQLScript/` 目錄下的 SQL Script：

   | 順序 | 檔案 | 說明 |
   |------|------|------|
   | 1 | `TSQL_Myoffice_ACPD.sql` | 建立主要資料表 |
   | 2 | `TSQL_Myoffice_ExcuteionLog.sql` | 建立執行日誌資料表 |
   | 3 | `NewSID_自訂一組固定欄位的代碼.sql` | 建立 NEWSID 預存程序（主鍵產生） |
   | 4 | `usp_AddLog 記錄執行錯誤.sql` | 建立 usp_AddLog 預存程序（流程記錄） |

---

## 啟動專案

1. 以 Visual Studio 2022 開啟 `Jason_BackendTest_MidLevel.sln`
2. 確認 `appsettings.json` 連線字串正確：

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=JASONHUANG;Database=Myoffice_ACPD;Integrated Security=True;TrustServerCertificate=True;"
   }
   ```

3. 按 **F5** 啟動，瀏覽器自動開啟 **Swagger UI**

---

## API 端點

Base URL：`https://localhost:{port}/api/myofficeacpd`

| Method | URL | 說明 | 成功回應 |
|--------|-----|------|----------|
| GET | `/api/myofficeacpd` | 查詢所有資料 | 200 OK |
| GET | `/api/myofficeacpd/{id}` | 查詢單筆資料 | 200 OK |
| POST | `/api/myofficeacpd` | 新增資料 | 201 Created |
| PUT | `/api/myofficeacpd/{id}` | 更新資料 | 200 OK |
| DELETE | `/api/myofficeacpd/{id}` | 刪除資料 | 204 No Content |

### HTTP Status Code 說明

| Code | 說明 |
|------|------|
| 200 | 請求成功 |
| 201 | 資源建立成功（POST） |
| 204 | 成功但無回傳內容（DELETE） |
| 400 | 請求參數有誤 |
| 404 | 資源不存在 |
| 500 | 伺服器內部錯誤 |

---

## Swagger 測試 JSON

### POST（新增）

```json
{
  "ACPD_Cname":    "王小明",
  "ACPD_Ename":    "Wang Xiao Ming",
  "ACPD_Sname":    "小明",
  "ACPD_Email":    "xiaoming@example.com",
  "ACPD_Status":   0,
  "ACPD_Stop":     false,
  "ACPD_StopMemo": null,
  "ACPD_LoginID":  "xiaoming",
  "ACPD_LoginPWD": "P@ssw0rd123",
  "ACPD_Memo":     "測試新增資料",
  "ACPD_NowID":    "ADMIN"
}
```

### PUT（更新）

```json
{
  "ACPD_Cname":    "王小明（已更新）",
  "ACPD_Ename":    "Wang Xiao Ming Updated",
  "ACPD_Sname":    "小明U",
  "ACPD_Email":    "updated@example.com",
  "ACPD_Status":   1,
  "ACPD_Stop":     false,
  "ACPD_StopMemo": null,
  "ACPD_LoginID":  "xiaoming_v2",
  "ACPD_LoginPWD": "NewP@ss456",
  "ACPD_Memo":     "測試更新資料",
  "ACPD_UPDID":    "ADMIN"
}
```

---

## 執行日誌（usp_AddLog）

每次 API 呼叫都會透過 `usp_AddLog` 記錄執行流程至 `MyOffice_ExcuteionLog`。
同一次請求共用同一個 `GroupID (GUID)`，可查詢整個執行歷程：

```sql
SELECT * FROM MyOffice_ExcuteionLog
WHERE DeLog_GroupID = '你的 GroupID'
ORDER BY DeLog_AutoID
```

### Debug 模式

在 Postman 加入請求標頭 `Jason_Debug: 1`，
可將 `DeLog_isCustomDebug = 1` 記錄至日誌，用於區分開發者手動測試的紀錄。

---

## 專案架構

```
Jason_BackendTest_MidLevel/
├── Controllers/
│   └── MyOfficeAcpdController.cs   # RESTful API 端點
├── Helpers/
│   ├── ILogHelper.cs               # Log 介面
│   └── LogHelper.cs                # usp_AddLog 封裝實作
├── Models/
│   ├── MyOfficeAcpd.cs             # 資料表 Entity
│   └── Dtos/
│       ├── CreateAcpdDto.cs        # 新增請求 DTO
│       └── UpdateAcpdDto.cs        # 更新請求 DTO
├── Repositories/
│   ├── Interfaces/
│   │   └── IMyOfficeAcpdRepository.cs
│   └── MyOfficeAcpdRepository.cs   # CRUD + NEWSID 實作
├── SwaggerExamples/
│   ├── CreateAcpdDtoExample.cs     # POST 範例 JSON
│   └── UpdateAcpdDtoExample.cs     # PUT 範例 JSON
├── appsettings.json                # 連線字串設定
└── Program.cs                      # DI 注冊 + Swagger 設定
TSQLScript/
├── TSQL_Myoffice_ACPD.sql
├── TSQL_Myoffice_ExcuteionLog.sql
├── NewSID_自訂一組固定欄位的代碼.sql
└── usp_AddLog 記錄執行錯誤.sql
```

---

## Git 分支策略

| 分支 | 說明 |
|------|------|
| `main` | 最終穩定版本 |
| `develop` | 開發整合主分支 |
| `feature/phase1-infrastructure` | 基礎架構建立 |
| `feature/phase2-repository` | Repository + LogHelper |
| `feature/phase3-controller` | Controller |
| `feature/phase4-swagger-readme` | Swagger + README |
