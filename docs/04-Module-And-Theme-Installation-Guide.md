# Document 4: Installation Guide for Modules and Themes

## Overview
This document covers the end-to-end procedures for packaging, installing, and activating new custom modules and themes in Oqtane Framework (.NET 10). It supports both UI-based self-service administration and automated CI/CD deployment pipelines.

---

## 1. Packaging Modules and Themes
Oqtane leverages the standard NuGet package format (`.nupkg`) as its universal distribution unit for modules and themes.

### Step 1: Create the `.nuspec` Manifest
Create a package manifest in the module project folder (e.g., `OqtanePoc.ProductCatalog.nuspec`):
```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>OqtanePoc.ProductCatalog</id>
    <version>1.0.0</version>
    <authors>Enterprise Engineering Team</authors>
    <description>Product Catalog Module with Isolated Tenant Database Support</description>
    <dependencies>
      <dependency id="Oqtane.Server" version="10.3.1" />
    </dependencies>
  </metadata>
  <files>
    <file src="bin\Release\net10.0\OqtanePoc.ProductCatalog.Server.Oqtane.dll" target="lib\net10.0" />
    <file src="bin\Release\net10.0\OqtanePoc.ProductCatalog.Client.Oqtane.dll" target="lib\net10.0" />
    <file src="bin\Release\net10.0\OqtanePoc.ProductCatalog.Shared.Oqtane.dll" target="lib\net10.0" />
    <file src="wwwroot\**\*.*" target="wwwroot" />
  </files>
</package>
```

### Step 2: Build and Pack
Execute from the terminal:
```powershell
dotnet build -c Release
dotnet pack -c Release
```
This generates `OqtanePoc.ProductCatalog.1.0.0.nupkg`.

---

## 2. Installation Methods

### Method 1: Web UI Package Manager (Recommended for Administrators)
This method requires no command line access or server restarts:
1. Log into Oqtane as a **Host** user (`http://localhost:48807`).
2. Open the **Control Panel** (gear icon) and click **Admin Dashboard**.
3. Under **System Administration**:
   - For Modules: Navigate to **Module Management**.
   - For Themes: Navigate to **Theme Management**.
4. Click **Install Module** (or **Install Theme**).
5. Drag and drop or browse to select the `.nupkg` file.
6. Click **Install**.
7. Oqtane will automatically:
   - Extract DLLs into the application `/bin` directory.
   - Extract static files into `/wwwroot`.
   - Register the module definition in `Oqtane_Master.dbo.ModuleDefinition`.
   - Trigger assembly reloading without taking the server down.

---

### Method 2: Direct Binary Drop (Recommended for CI/CD Pipelines)
For DevOps automated deployment pipelines:
1. Compile the module or theme solution in Release mode.
2. Stop the host process (e.g. `Stop-WebAppPool -Name "OqtanePool"` or stop Kestrel).
3. Copy compiled binaries:
   - Copy `*.Server.Oqtane.dll`, `*.Client.Oqtane.dll`, `*.Shared.Oqtane.dll` into the web root `/bin` folder.
4. Copy static assets:
   - Copy CSS/JS/images to `/wwwroot/Modules/{ModuleName}/` or `/wwwroot/Themes/{ThemeName}/`.
5. Start the web application.
6. On startup, Oqtane's dynamic assembly loader scans all DLLs ending with `.Oqtane.dll`, discovers the `IModule` and `IInstallable` interfaces, and installs the components across the master and tenant databases.

---

### Method 3: Visual Studio Solution Reference (Development Environment)
During local development:
1. Add the module project to the solution (`Oqtane.sln`).
2. Add a `<ProjectReference>` inside `Server/OqtanePoc.Server.csproj` pointing to the module's Server project.
3. Build and launch (`dotnet run`).
4. The module/theme is immediately active and discoverable in the Control Panel.

---

## 3. Adding Modules and Themes to a Specific Tenant

### Adding a Module to a Page:
1. Navigate to the desired Tenant site (e.g. `http://localhost:48807/tenant2`).
2. Log in as an Administrator.
3. Open the **Control Panel** (offcanvas drawer).
4. Under **Module Management**:
   - Set **Category** dropdown to `Common Modules`.
   - Select the desired module from `<Select Module>`.
   - Set **Title**, **Pane** (e.g., `Default Pane`), and **Location**.
5. Click **Add Module to Page**.
6. Oqtane triggers `IInstallable.Install(tenant)` and renders the component.

### Applying a Theme to a Single Tenant:
1. Navigate to the target Tenant site (e.g. `http://localhost:48807`).
2. Log in as an Administrator.
3. Navigate to **Control Panel > Admin > Site Settings**.
4. Expand the **Theme** section:
   - Set **Default Theme** to your custom theme (e.g., `Enterprise Modern Theme - Default`).
   - Set **Default Container** to your custom container (e.g., `Enterprise Modern Container`).
5. Click **Save**.
6. The theme is applied exclusively to this tenant, while other tenants retain their default themes.
