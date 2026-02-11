# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CGBC Online Directory — an ASP.NET WebForms 4.8 application for managing a church member directory. Members can view the directory; administrators can manage members, users, and contacts.

**Stack:** C# / .NET Framework 4.8 / ASP.NET WebForms / Telerik RadControls 2023.3.1010.45 / SQL Server

## Build and Run

```bash
# Build the solution
msbuild Directory.CGBC.sln

# Build a specific configuration
msbuild Directory.CGBC.sln /p:Configuration=Debug
msbuild Directory.CGBC.sln /p:Configuration=Release
```

The application runs on IIS Express (port 49573) via Visual Studio. No automated tests or linters are configured.

## Solution Structure

Two active projects in `Directory.CGBC.sln`:

- **Directory.CGBC/** — The web application (ASP.NET 4.8). Contains ASPX pages, code-behind, domain objects, and data access.
- **GloryKidd.WebCore/** — Shared class library (.NET 4.7.2). Contains base classes, SQL helpers, encryption, session management, and string extension methods.
- **lib/RCAJAX/** — Bundled Telerik RadControls DLLs (no NuGet).

A third project `Directory.Setup` (WiX installer) is referenced in the solution but not present in this workspace.

## Architecture

**Three-tier with session-based state:**

1. **Presentation:** ASPX pages with Telerik RadControls, inheriting from `BasePage`/`BaseMasterPage` in WebCore. Two master pages: `MasterPage.Master` (login flow) and `PageMaster.Master` (authenticated pages).
2. **Business Logic:** Domain objects in `Directory.CGBC/Objects/` — `Member.cs` (member data, relations, notes) and `TypeObjects.cs` (Address, RelatedMember, etc.). User auth via `SystemUser.cs` in WebCore.
3. **Data Access:** `SqlHelpers.cs` executes parameterized queries. `SqlStatements.cs` defines all SQL as string constants. `SqlDataLoader.cs` provides static caching for lookup data (states, salutations, etc.).

**Key patterns:**
- **Singleton via session:** `SystemUser.StaticInstance` and `SystemSettings.StaticInstance` store per-session state.
- **Static lookup caching:** `SqlDataLoader` lazy-loads and caches reference data in static collections.
- **Extension methods:** `StringHelpers.cs` in WebCore provides SQL safety (`FixSqlString`), type conversions (`GetInt32`, `GetAsDate`), formatting (`FormatPhone`), and encryption (`EncryptString`/`DecryptString`).
- **Encrypted connection string:** The database connection string in `web.config` is AES-encrypted; `EncryptionHelper.cs` handles decryption at runtime.

## Page Flow

- **Default.aspx** — Login entry point. Authenticates via `SystemUser.AuthenticateUser()`, redirects to MainDirectory.
- **MainDirectory.aspx** — Primary page. Displays all members in a searchable/expandable RadGrid. Admin users see "New Member" button; super-admins see admin link.
- **Admin/EditMember.aspx** — Add/edit member details, related members, and notes (admin only).
- **Admin/AdminUsers.aspx** — Manage system users and roles (super-admin only).
- **ForgotPassword.aspx** — Password reset request.
- **PrintMember.aspx** — Printable member view.

## Database

SQL Server with tables: `Member`, `AdminUsers`, `AdminRoles`, `States`, `Salutation`, `MaritalStatus`, `RelationshipType`, `MemberNotes`, `Xref_Member_Member` (member relationships), `SystemConfigs`, `SystemExceptions`, `PageLocations`, `PageContent`.

All SQL queries are defined as constants in `GloryKidd.WebCore/Helpers/SqlStatements.cs`.
