# CGBC Online Directory — Blazor Server Application

A .NET 10 Blazor Server application for managing the Cedar Grove Baptist Church member directory. Members can view the directory; administrators can manage members, users, and contacts. This is a full rewrite of the legacy ASP.NET WebForms 4.8 application using modern technologies.

## Technology Stack

| Component | Technology |
|---|---|
| Framework | .NET 10 |
| UI | Blazor Server (SSR + SignalR) |
| UI Components | BlazorBootstrap 3.5 (Bootstrap 5 wrapper) |
| Authentication | ASP.NET Core Identity |
| Database ORM | Entity Framework Core 10 (Code-First) |
| Database | SQL Server |
| PDF Generation | QuestPDF 2025.12 (server-side) |
| Architecture | Clean Architecture (4 projects) |

## Solution Structure

```
Directory.Blazor.sln
src/
  Directory.Domain/            # Entities, enums, interfaces — zero dependencies
  Directory.Application/       # DTOs, service interfaces, service implementations, helpers
  Directory.Infrastructure/    # EF DbContext, Identity, email service, seed data
  Directory.Web/               # Blazor Server app, pages, components, DI setup
```

### Project Dependency Graph

```
Directory.Web
  ├── Directory.Application
  │     └── Directory.Domain
  └── Directory.Infrastructure
        ├── Directory.Application
        └── Directory.Domain
```

---

## Architecture Details

### Directory.Domain

The domain layer has no external dependencies. It contains:

**Entities** (`Entities/`):
- `BaseEntity` — abstract base class with `int Id`
- `Member` — core entity with name, address, contact, and date fields. Computed `DisplayName` property constructs the full name from Salutation + First + Middle + Last + Suffix
- `MemberRelationship` — junction table linking two Members with a RelationshipType. Has two foreign keys to Member (`MemberId`, `RelatedMemberId`)
- `MemberNote` — append-only notes attached to a member, tracks the `UserId` (Identity user) who created it
- `State`, `Salutation`, `MaritalStatus`, `RelationshipType` — lookup entities implementing `ILookupEntity`
- `SystemSetting` — single-row SMTP configuration table
- `SystemException` — error logging table

**Enums** (`Enums/`):
- `Gender` — Male (0), Female (1), matching the legacy database values

**Interfaces** (`Interfaces/`):
- `ILookupEntity` — contract for lookup tables (`Id`, `Name`)

### Directory.Application

The application layer depends only on Domain and `Microsoft.EntityFrameworkCore` (for `DbSet<T>` in the context interface). It contains:

**DTOs** (`DTOs/`):

| DTO | Purpose |
|---|---|
| `MemberListDto` | Grid view: Id, FirstName, LastName, CellPhone, HomePhone, LastUpdate |
| `MemberDetailDto` | Expanded row: DisplayName, formatted address, phones, emails, dates, related members, notes |
| `MemberDto` | Full edit form with all fields including related members and notes collections |
| `RelatedMemberDto` | MemberId, DisplayName, RelationshipTypeId, RelationshipTypeName |
| `MemberNoteDto` | Id, UserName, NoteText, CreatedDate |
| `UserDto` | Id, Email, DisplayName, Role, MemberId, MemberName, IsSuperAdmin |
| `LookupDto` | Generic Id/Name pair for dropdowns |
| `SystemSettingDto` | SMTP configuration fields |
| `SystemExceptionDto` | Error log fields |

**Service Interfaces** (`Interfaces/`):

| Interface | Key Methods |
|---|---|
| `IMemberService` | `GetDirectoryListAsync`, `GetMemberDetailAsync`, `GetMemberForEditAsync`, `CreateAsync`, `UpdateAsync`, `SaveRelationshipsAsync`, `DeleteAsync` |
| `IUserService` | `GetAllUsersAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `ResetPasswordAsync`, `ChangePasswordAsync` |
| `IEmailService` | `SendPasswordResetEmailAsync` |
| `ILookupService` | `GetStatesAsync`, `GetSalutationsAsync`, `GetMaritalStatusesAsync`, `GetRelationshipTypesAsync`, `GetAllMembersLookupAsync` |
| `ISystemSettingService` | `GetSettingsAsync`, `UpdateSettingsAsync` |
| `ISystemExceptionService` | `GetAllAsync`, `LogAsync` |
| `IApplicationDbContext` | Abstraction over `ApplicationDbContext` with all `DbSet<T>` properties |
| `IUserNameResolver` | `GetDisplayNameAsync(userId)` — resolves Identity user IDs to display names |

**Service Implementations** (`Services/`):

- `MemberService` — handles all member CRUD and the critical **bidirectional relationship logic**
- `LookupService` — loads lookup data for dropdowns
- `SystemSettingService` — single-row SMTP settings CRUD
- `SystemExceptionService` — error log query and insert

**Helpers** (`Helpers/`):

- `FormatHelpers` — phone formatting (10-digit to `(NPA) NXX-XXXX`, 7-digit to `NXX-XXXX`), phone/zip cleaning (digits only), address formatting (multi-line string)
- `MappingExtensions` — manual entity-to-DTO mapping methods (`ToListDto`, `ToDetailDto`, `ToEditDto`, `ApplyToEntity`, `ToNoteDto`, `ToDto`)

### Directory.Infrastructure

The infrastructure layer depends on Domain, Application, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, and `Microsoft.EntityFrameworkCore.SqlServer`.

**Identity** (`Identity/`):
- `ApplicationUser` extends `IdentityUser` with `DisplayName`, `MemberId` (nullable FK to Member), and `IsSuperAdmin`

**Data** (`Data/`):
- `ApplicationDbContext` extends `IdentityDbContext<ApplicationUser>` and implements `IApplicationDbContext`. Applies all Fluent API configurations from assembly

**EF Configurations** (`Data/Configurations/`):

| Configuration | Key Decisions |
|---|---|
| `MemberConfiguration` | Table `Members`, `DisplayName` ignored (computed), FK to Salutation/MaritalStatus/State with `Restrict` delete |
| `MemberRelationshipConfiguration` | Table `MemberRelationships`, unique index on `(MemberId, RelatedMemberId)`, both FKs to Member with `Restrict` delete |
| `MemberNoteConfiguration` | Table `MemberNotes`, FK to Member with `Cascade` delete |
| `ApplicationUserConfiguration` | FK to Member with `SetNull` delete |
| Lookup configs | Tables: `States`, `Salutations`, `MaritalStatuses`, `RelationshipTypes` |
| `SystemSettingConfiguration` | Table `SystemSettings` |
| `SystemExceptionConfiguration` | Table `SystemExceptions` |

**Seed Data** (`Data/SeedData.cs`):

Runs on startup via `SeedData.InitializeAsync()`:
1. Applies pending EF migrations
2. Creates roles: `User`, `Admin`, `SuperAdmin`
3. Seeds lookup tables:
   - 6 Salutations (Mr., Mrs., Ms., Dr., Rev., Miss)
   - 5 Marital Statuses (Single, Married, Divorced, Widowed, Separated)
   - 4 Relationship Types (Spouse=1, Child=2, Sibling=3, Parent=4) — IDs match the legacy system
   - 51 States (all 50 + District of Columbia)
4. Creates default `SystemSettings` row with placeholder SMTP config
5. Creates default SuperAdmin account: `admin@cedargrovebaptist.church` / `ChangeMe123!`

**Services** (`Services/`):
- `UserService` — wraps `UserManager<ApplicationUser>` for user CRUD, role management, password reset (generates 10-char random password), and password change
- `EmailService` — reads SMTP config from `SystemSettings`, sends password reset emails matching the legacy format
- `UserNameResolver` — resolves Identity user IDs to display names for member note attribution

### Directory.Web

The Blazor Server web application. References all three other projects.

**Program.cs** — Startup configuration:
- Registers `ApplicationDbContext` with SQL Server provider
- Configures ASP.NET Identity with password requirements (8+ chars, digit, lowercase, uppercase, special char)
- Cookie auth redirects to `/login`, 8-hour expiration with sliding
- Registers all application services as scoped
- Configures Blazor Server with InteractiveServer render mode and BlazorBootstrap
- Runs seed data on startup
- Maps a POST `/account/logout` endpoint for sign-out

**Layouts** (`Components/Layout/`):

| Layout | Replaces | Usage |
|---|---|---|
| `MainLayout.razor` | `PageMaster.Master` | Authenticated pages: NavHeader + content + footer |
| `LoginLayout.razor` | `MasterPage.Master` | Centered card layout for login/forgot-password |
| `NavHeader.razor` | Duplicated header markup | App title, "Welcome {name}", Admin link (SuperAdmin), Change Password, Logout |

**Pages** (`Components/Pages/`):

| Page | Route(s) | Replaces | Description |
|---|---|---|---|
| `Login.razor` | `/login`, `/` | `Default.aspx` | Email + password form, redirects to `/directory` on success |
| `ForgotPassword.razor` | `/forgot-password` | `ForgotPassword.aspx` | Email input, generates temp password, sends reset email. Always shows "check your email" regardless of account existence |
| `MemberDirectory.razor` | `/directory` | `MainDirectory.aspx` | Searchable/sortable/pageable member table (50 per page). Click row to expand detail panel. Edit link visible for Admin/SuperAdmin or own linked member |
| `EditMember.razor` | `/members/new`, `/members/edit/{id}` | `Admin/EditMember.aspx` | Full member edit form with personal info, contact info, related members grid, and notes. Authorization checks: Admin/SuperAdmin or own member |
| `PrintMember.razor` | `/members/print/{id}` | `PrintMember.aspx` | Read-only member detail with Print and Export PDF buttons |
| `AdminUsers.razor` | `/admin/users` | `Admin/AdminUsers.aspx` | SuperAdmin only. User grid with inline edit, add modal, delete confirmation, password reset |
| `SystemSettings.razor` | `/admin/settings` | (new) | SuperAdmin only. SMTP configuration form |
| `SystemExceptions.razor` | `/admin/errors` | (new) | SuperAdmin only. Paginated error log with expandable stack traces |

**Shared Components** (`Components/Shared/`):

| Component | Description |
|---|---|
| `MemberDetailPanel.razor` | Card displaying full member details, related members, notes, and print link. Used in directory expansion and print page |
| `RelatedMembersGrid.razor` | Editable table of member relationships with add/remove. Member dropdown, relationship type dropdown |
| `MemberNotesDisplay.razor` | Read-only chronological list of notes with user name and date |
| `ChangePasswordModal.razor` | Modal form: current password, new password, confirm. Uses `UserManager.ChangePasswordAsync` |
| `ConfirmModal.razor` | Generic confirmation dialog with customizable title, message, and confirm button |
| `PhoneDisplay.razor` | Formats and displays a phone number |

**Helpers** (`Helpers/`):
- `PdfGenerator.cs` — static method `GenerateMemberPdf` using QuestPDF. Produces a Letter-sized PDF with member name, address, contacts, dates, related members, and notes

**Static Assets** (`wwwroot/`):
- `js/download.js` — JS interop function to trigger browser file download from base64 data

---

## Key Business Logic

### Bidirectional Relationship Saving

When saving member relationships (replicated from legacy `SqlDataLoader.SaveMemberRelations`):

1. Delete **all** existing relationships for the member in both directions (where `MemberId = X` OR `RelatedMemberId = X`)
2. For each relationship in the submitted list:
   - Insert the primary relationship: `(MemberId, RelatedMemberId, TypeId)`
   - Insert the **reciprocal** relationship with mirrored type:

| Primary Type | Reciprocal Type |
|---|---|
| Spouse (1) | Spouse (1) |
| Child (2) | Parent (4) |
| Sibling (3) | Sibling (3) |
| Parent (4) | Child (2) |

This is implemented in `MemberService.SaveRelationshipsAsync()`.

### Edit Authorization

A member record can be edited by:
- Any user with the `Admin` or `SuperAdmin` role
- A regular `User` whose `ApplicationUser.MemberId` matches the member being edited

This check appears in both `MemberDirectory.razor` (edit button visibility) and `EditMember.razor` (page-level authorization).

### Phone Number Handling

- **Storage**: digits only, max 10 characters (stripped of formatting characters)
- **Display**: formatted as `(NPA) NXX-XXXX` for 10 digits, `NXX-XXXX` for 7 digits, raw for other lengths
- Implemented in `FormatHelpers.FormatPhone()` and `FormatHelpers.CleanPhone()`

### Notes

Member notes are **append-only** — they are never edited or deleted (only inserted). Each note records the creating user's ID and timestamp.

### Password Reset Security

The forgot-password flow always displays "If an account exists with that email address, a password reset email has been sent" — regardless of whether the account exists. This prevents account enumeration.

---

## Roles

| Role | Capabilities |
|---|---|
| `User` | View directory, expand member details, edit own linked member profile, change own password |
| `Admin` | All User capabilities + create/edit any member |
| `SuperAdmin` | All Admin capabilities + user administration, system settings, error log viewing |

---

## Database Schema

The EF Code-First model produces these tables:

### Application Tables

| Table | Description |
|---|---|
| `Members` | Core member data (name, address, contact, dates) |
| `MemberRelationships` | Many-to-many member relationships with type. Unique index on `(MemberId, RelatedMemberId)` |
| `MemberNotes` | Append-only notes per member. Cascades on member delete |
| `States` | US states + DC (51 rows) |
| `Salutations` | Mr., Mrs., Ms., Dr., Rev., Miss |
| `MaritalStatuses` | Single, Married, Divorced, Widowed, Separated |
| `RelationshipTypes` | Spouse (1), Child (2), Sibling (3), Parent (4) |
| `SystemSettings` | Single-row SMTP configuration |
| `SystemExceptions` | Error log |

### Identity Tables (auto-generated)

| Table | Description |
|---|---|
| `AspNetUsers` | Extended with `DisplayName`, `MemberId` (FK to Members, SetNull), `IsSuperAdmin` |
| `AspNetRoles` | User, Admin, SuperAdmin |
| `AspNetUserRoles` | User-to-role mapping |
| `AspNetUserClaims` | (standard Identity) |
| `AspNetUserLogins` | (standard Identity) |
| `AspNetUserTokens` | (standard Identity) |
| `AspNetRoleClaims` | (standard Identity) |

---

## Component Mapping (Legacy to New)

| Legacy (Telerik/WebForms) | New (Blazor/BlazorBootstrap) |
|---|---|
| RadGrid | HTML `<table>` with manual sorting/filtering/paging |
| RadGrid NestedView | `MemberDetailPanel` component expanded below row |
| RadGrid InPlace Edit | Conditional template rendering (edit/display toggle) |
| RadDropDownList | `<select class="form-select">` / `<InputSelect>` |
| RadComboBox | `<select>` with filtered options |
| RadTextBox | `<InputText class="form-control">` |
| RadMaskedTextBox | `<InputText>` with `FormatHelpers` |
| RadDatePicker | `<InputDate>` |
| RadButton | `<Button>` (BlazorBootstrap) or `<button>` |
| RadWindow (modal) | `<Modal>` (BlazorBootstrap) |
| RadPageLayout | Bootstrap 5 `row`/`col-*` grid |
| RadAjaxPanel | Blazor SignalR interactivity (built-in) |
| RadClientExportManager | QuestPDF + JS interop download |
| ASP.NET Validators | `<DataAnnotationsValidator>` + `<ValidationMessage>` |
| AES-encrypted passwords | ASP.NET Identity password hashing |
| Session-based `SystemUser` singleton | `AuthenticationStateProvider` + `UserManager<ApplicationUser>` |
| `MasterPage.Master` (login) | `LoginLayout.razor` |
| `PageMaster.Master` (authenticated) | `MainLayout.razor` + `NavHeader.razor` |

---

## Page Flow

```
/ or /login
  ├── Login form
  │     └── Success → /directory
  └── /forgot-password
        └── Reset email sent → /login

/directory (authenticated)
  ├── Search/filter/sort/page member grid
  ├── Click row → expand MemberDetailPanel
  │     └── Print link → /members/print/{id}
  ├── Edit link → /members/edit/{id}
  └── New Member button (Admin+) → /members/new

/members/edit/{id} or /members/new (authenticated, authorized)
  ├── Personal info form (salutation, name, gender, marital status, dates)
  ├── Contact info form (address, phones, emails)
  ├── Related members grid (add/remove)
  ├── Notes (add new, view history)
  └── Save → redirect or reload

/members/print/{id} (authenticated)
  ├── MemberDetailPanel (read-only)
  ├── Print button (window.print)
  └── Export PDF button (QuestPDF → browser download)

/admin/users (SuperAdmin)
  ├── User grid with inline edit
  ├── Add User modal
  ├── Reset Password → email with temp password
  ├── Delete User with confirmation
  ├── → /admin/settings
  └── → /admin/errors

/admin/settings (SuperAdmin)
  └── SMTP configuration form

/admin/errors (SuperAdmin)
  └── Paginated error log with expandable stack traces
```

---

## Build and Run

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB, Express, or full instance)

### Build

```bash
dotnet build Directory.Blazor.sln
```

### Create Initial Migration

```bash
dotnet ef migrations add InitialCreate \
  --project src/Directory.Infrastructure \
  --startup-project src/Directory.Web
```

### Apply Migration (optional — also runs automatically on startup)

```bash
dotnet ef database update \
  --project src/Directory.Infrastructure \
  --startup-project src/Directory.Web
```

### Run

```bash
# With hot reload
dotnet watch run --project src/Directory.Web

# Without hot reload
dotnet run --project src/Directory.Web
```

The app will be available at `https://localhost:5001` (or the port shown in console output).

### Default Login

| Field | Value |
|---|---|
| Email | `admin@cedargrovebaptist.church` |
| Password | `ChangeMe123!` |
| Role | SuperAdmin |

### Subsequent Migrations

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Directory.Infrastructure \
  --startup-project src/Directory.Web
```

---

## Configuration

### Connection String

Edit `src/Directory.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CGBCDirectory;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

For LocalDB (default):
```
Server=(localdb)\mssqllocaldb;Database=CGBCDirectory;Trusted_Connection=True;MultipleActiveResultSets=true
```

For SQL Server with authentication:
```
Server=your-server;Database=CGBCDirectory;User Id=your-user;Password=your-password;MultipleActiveResultSets=true;TrustServerCertificate=True
```

### SMTP Settings

After first login, navigate to `/admin/settings` to configure:
- Mail server hostname and port
- SMTP credentials
- From email and display name
- SSL and authentication requirements

---

## Next Steps

### Before First Run

1. **Update the connection string** in `src/Directory.Web/appsettings.json` to point to your SQL Server instance
2. **Create the initial EF migration**: `dotnet ef migrations add InitialCreate --project src/Directory.Infrastructure --startup-project src/Directory.Web`
3. **Run the application**: `dotnet run --project src/Directory.Web`
4. **Log in** with the default SuperAdmin account and change the password immediately
5. **Configure SMTP** at `/admin/settings` to enable password reset emails

### Data Migration (if needed)

The application starts with a fresh database. If data migration from the legacy system is needed:
- Export member data from the legacy SQL Server tables
- Map legacy `AdminUsers` to Identity users (passwords will need to be reset since the legacy system used custom AES encryption)
- Map legacy `Xref_Member_Member` relationships to `MemberRelationships`
- Map legacy `MemberNotes` to the new schema (note: `UserId` changes from `int` to Identity `string`)
- Seed data IDs for lookup tables (States, Salutations, MaritalStatuses, RelationshipTypes) match the legacy system by design

### Potential Enhancements

- **Global error handling** — wire up `ISystemExceptionService.LogAsync` in a middleware or Blazor error boundary
- **Member photo upload** — add a photo field to Member and file storage
- **Audit trail** — track who changed what and when
- **Export directory** — PDF or CSV export of the full directory
- **Search improvements** — full-text search across all member fields
- **Responsive grid** — replace the HTML table with BlazorBootstrap Grid component for built-in sorting/filtering/paging
- **Email templates** — configurable email templates instead of hardcoded HTML
- **Unit tests** — test services against an in-memory EF provider
- **Docker support** — add `Dockerfile` and `docker-compose.yml` for containerized deployment

---

## Legacy Application Reference

The original application (`Directory.CGBC.sln`) remains in the repository alongside the new Blazor solution. Key files for reference:

| Legacy File | Purpose |
|---|---|
| `Directory.CGBC/Helpers/SqlDataLoader.cs` | Bidirectional relationship logic (lines 45-71) |
| `Directory.CGBC/Objects/Member.cs` | Member entity and DisplayName construction |
| `Directory.CGBC/Objects/TypeObjects.cs` | Address, RelatedMember, MemberNote, lookup types |
| `Directory.CGBC/MainDirectory.aspx.cs` | Directory page behavior, edit authorization |
| `Directory.CGBC/Admin/EditMember.aspx.cs` | Member edit/save workflow |
| `Directory.CGBC/Admin/AdminUsers.aspx.cs` | User management with inline grid edit |
| `GloryKidd.WebCore/BaseObjects/SystemUser.cs` | Auth, user CRUD, password management |
| `GloryKidd.WebCore/Helpers/SessionManager.cs` | Email sending, session state |
| `GloryKidd.WebCore/Helpers/StringHelpers.cs` | Phone/address formatting (ported to `FormatHelpers`) |
| `GloryKidd.WebCore/Helpers/SqlStatements.cs` | All SQL queries (schema reference) |
