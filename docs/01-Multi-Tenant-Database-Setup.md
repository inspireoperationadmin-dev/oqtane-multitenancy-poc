# Document 1: Multi-Tenant Database Setup Guide

## 1. Architectural Overview
Oqtane Framework utilizes a multi-database architecture where each tenant operates on its own dedicated database, managed via a global **Master Database**.

```
[ Incoming HTTP Request ] 
            │
            ▼
[ Oqtane TenantMiddleware ] (Resolves Domain / Alias)
            │
            ├───────────────┬───────────────┐
            ▼               ▼               ▼
    [ Tenant 1 DB ]  [ Tenant 2 DB ]  [ Tenant 3 DB ]
    (ProductCatalog)   (HelpDesk)       (Feedback)
            ▲               ▲               ▲
            └───────────────┼───────────────┘
                            │
                  [ Master Database ]
             (Tenants, Aliases, Modules)
```

---

## 2. Databases Configured in this POC

| Database Name | Tenant Name | URL Alias | Purpose |
| :--- | :--- | :--- | :--- |
| **`Oqtane_Master`** | Master | `localhost:48807` | Tenant registry, URL routing, job schedules |
| **`Oqtane-202611161034`** | Master / Tenant 1 | `localhost:48807` | Primary tenant site, Product Catalog module |
| **`Oqtane_Tenant2`** | Tenant2 | `localhost:48807/tenant2` | HelpDesk Support Portal module |
| **`Oqtane_Tenant3`** | Tenant3 | `localhost:48807/tenant3` | Customer Feedback & Surveys module |

---

## 3. How to Provision New Tenants (Step-by-Step)

### Via Oqtane 10 Admin UI:
1. Log in as `host` at `http://localhost:48807`.
2. Open **Control Panel (Gear Icon) > Admin Dashboard > Site Management**.
3. Click **Add Site**:
   - **Site Name**: Enter site name (e.g., `HelpDesk Portal`).
   - **Url**: Enter unique alias path (e.g., `localhost:48807/tenant2`).
   - **Site Template**: Select `Default Site Template` or `Empty Site Template`.
   - **Database**: Select **`<Add Database>`** from the dropdown.
4. When `<Add Database>` is selected, complete the expanded fields:
   - **Name (Tenant Name)**: `Tenant2`
   - **Database Type**: `LocalDB` (or SQL Server / SQLite / PostgreSQL / MySQL)
   - **Database Name**: `Oqtane_Tenant2`
   - **Host Username / Password**: Enter host administrator credentials.
5. Click **Save**.
6. **Result**: Oqtane creates the database, provisions system tables (`Site`, `Page`, `Module`, `User`, `Role`), and links the URL alias automatically.

---

## 4. Maintenance & Verification
- **Verifying in SQL Server**:
  Open SQL Server Management Studio (SSMS) or Visual Studio SQL Object Explorer connecting to `(localdb)\MSSQLLocalDB`. All tenant databases are listed as distinct, independent databases.
- **Connection Isolation**:
  Because connection strings are resolved per request via `ITenantManager`, code executing within Tenant 1 physically cannot access or query data in Tenant 2 or Tenant 3.
