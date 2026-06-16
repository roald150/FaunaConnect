# faunaConnect2 Academic Audit & Status Registry

**Date:** June 16, 2026  
**Auditor:** Gemini CLI  
**Focus:** Agile realization of a complex information system (Learning Outcome 3)

---

## 1. Tech Stack Compliance (.NET 10 / C# 14)
- **[ COMPLIANCE STATUS ]** 100% Exam-Compliant
- **[ INVENTORY OF FILES ]**
  - `FaunaConnect2.Api/FaunaConnect2.Api.csproj` (Targets `net10.0`)
  - `FaunaConnect2.App/FaunaConnect2.App.csproj` (Targets `net10.0`)
- **[ EXAM-GAP ACTION ITEMS ]**
  - [x] **Refactoring:** Leveraged C# 14 Primary Constructors across all API Controllers and MAUI ViewModels.
  - [x] **Collections:** Applied C# 12/14 collection expressions (`[]`) for list initializations.
  - [x] **MVVM Toolkit:** Integrated `CommunityToolkit.Mvvm` for source-generated properties and commands.

## 2. Internationalization (i18n)
- **[ COMPLIANCE STATUS ]** 100% - Rework Completed
- **[ INVENTORY OF FILES ]**
  - **Models:** `User.cs`, `Registration.cs`, `HuntingGround.cs` (English).
  - **Controllers:** All controllers fully translated.
  - **UI (XAML):** All major pages translated (English labels).
  - **Logic:** `WeatherHelper.cs` (Returns international cardinal points).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [x] **Data Model:** Rename `Jager` -> `Hunter`, `Boer` -> `Farmer`.
  - [x] **Variable Names:** Rename `isJagerOfBoer` to `isHunterOrFarmer` and similar occurrences.
  - [x] **UI Strings:** Migrate all hardcoded Dutch strings in XAML and `.xaml.cs` (alerts/displays) to English.
  - [x] **Comments:** Translate all Dutch technical comments into professional English.
  - [x] **Domain Logic:** Update `WeatherHelper.cs` to use international cardinal points (NE, SE, etc.).

## 3. MVVM Architectural Decoupling
- **[ COMPLIANCE STATUS ]** 100% - Fully Decoupled
- **[ INVENTORY OF FILES ]**
  - `ViewModels/`: `BaseViewModel`, `MainViewModel`, `MapViewModel`, `RegistrationViewModel`, `ChatViewModel`, `LoginViewModel`, `RegisterViewModel`, `DamageReportViewModel`, `AdminViewModel`, `NotificationsViewModel`, `RegistrationsViewModel`.
- **[ EXAM-GAP ACTION ITEMS ]**
  - [x] **Infrastructure:** Created a `ViewModels` folder and implemented a `BaseViewModel` leveraging `ObservableObject`.
  - [x] **Refactoring:** Extracted all HTTP calls, state management, and navigation logic from code-behind files into dedicated ViewModels.
  - [x] **Dependency Injection:** Configured `MauiProgram.cs` for full DI support of Services, ViewModels, and Pages.
  - [x] **Data Binding:** Replaced procedural UI updates with XAML Data Binding and compiled bindings (`x:DataType`).
  - [x] **Commanding:** Replaced event handlers with `[RelayCommand]` implementations in ViewModels.

## 4. Hardware & Local Persistence
- **[ COMPLIANCE STATUS ]** 100% - Fully Integrated
- **[ INVENTORY OF FILES ]**
  - **GPS/Camera Service:** `IDeviceService.cs` & `DeviceService.cs` (Decoupled hardware access).
  - **Spatial Logic:** `SpatialService.cs` (C# domain logic for safety radius and area checks).
  - **SQLite:** `LocalDatabaseService.cs` (Offline fallback for registrations and damage reports).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [x] **Persistence:** Integrated `sqlite-net-pcl` to allow offline registration of wildlife observations and damage reports.
  - [x] **Hardware Logic:** Implemented proper permission checking and error handling for GPS/Camera in a decoupled `DeviceService`.
  - [x] **Spatial Logic:** Refactored the 300m safety radius calculation and point-in-polygon logic into `SpatialService.cs`.

## 5. Backend, Security & Cloud Matrix
- **[ COMPLIANCE STATUS ]** 100% - Fully Secured
- **[ INVENTORY OF FILES ]**
  - **Persistence:** `FaunaDbContext.cs` (EF Core with SQL Server).
  - **Security:** `UsersController.cs` (BCrypt hashing and JWT generation).
  - **JWT:** `Program.cs` (JWT Bearer setup).
  - **Proxy:** `ParcelsController.cs` (Integration with PDOK/Kadaster API via `IHttpClientFactory`).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [x] **Authentication:** Implement JWT Bearer Token authentication in the API.
  - [x] **Authorization:** Apply `[Authorize(Roles = "...")]` attributes to all protected endpoints.
  - [x] **Password Security:** Replace plain-text password storage with secure BCrypt hashing.
  - [x] **User Service:** Refactored `IUserService` to handle token storage and authenticated client generation via DI.

## 6. Agile Framework Alignment
- **[ COMPLIANCE STATUS ]** Partial - Documentation Pending
- **[ INVENTORY OF FILES ]**
  - `README.md` (Technical overview).
  - `OPLEVERING.md` (Handover status).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Documentation:** Create `UserStories.md` mapping features to exam requirements.
  - [ ] **Governance:** Define a "Definition of Done" (DoD) in `DOD.md`.
  - [ ] **Process:** Add metadata to methods/classes linking them to specific User Stories (e.g., `[UserStory("US-01")]`).

## 7. Verification Engineering (Testing Suite)
- **[ COMPLIANCE STATUS ]** Partial - Requires Rework
- **[ INVENTORY OF FILES ]**
  - **Unit Tests:** `FaunaConnect2.Api/Tests/` (Contains 4 tests).
  - **UI Tests:** **Missing**.
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Unit Testing:** Increase coverage for `SpatialService` and `ViewModels` (facilitated by DI).
  - [ ] **UI Testing:** Implement at least 3 Appium or MAUI UI tests verifying the "Happy Path" for Hunter and Farmer roles.
  - [ ] **Acceptance:** Link tests to User Stories to prove DoD compliance.

---
**Audit Summary:** The project has been successfully modernized with .NET 10 / C# 14, a clean MVVM architecture, and decoupled hardware services. Security and persistence gaps have been closed. Remaining focus should be on Verification Engineering (Testing) and Agile documentation.
