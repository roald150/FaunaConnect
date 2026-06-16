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
  - [ ] Refactor existing code to leverage C# 14 features where appropriate (e.g., Primary Constructors, Enhanced collection expressions).

## 2. Internationalization (i18n)
- **[ COMPLIANCE STATUS ]** Partial - Requires Major Rework
- **[ INVENTORY OF FILES ]**
  - **Models:** `User.cs`, `Registration.cs`, `HuntingGround.cs` (Uses "Jager", "Boer").
  - **Controllers:** `ParcelsController.cs`, `RegistrationsController.cs` (Dutch comments and variable names).
  - **UI (XAML):** `MainPage.xaml`, `MapPage.xaml`, `DamageReportPage.xaml`, `AdminPage.xaml` (Hardcoded Dutch labels).
  - **Logic:** `WeatherHelper.cs` (Returns Dutch wind directions: "NO", "ZO", etc.).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Data Model:** Rename `Jager` -> `Hunter`, `Boer` -> `Farmer`.
  - [ ] **Variable Names:** Rename `isJagerOfBoer` to `isHunterOrFarmer` and similar occurrences.
  - [ ] **UI Strings:** Migrate all hardcoded Dutch strings in XAML and `.xaml.cs` (alerts/displays) to English.
  - [ ] **Comments:** Translate all Dutch technical comments into professional English.
  - [ ] **Domain Logic:** Update `WeatherHelper.cs` to use international cardinal points (NE, SE, etc.).

## 3. MVVM Architectural Decoupling
- **[ COMPLIANCE STATUS ]** Missing Entirely
- **[ INVENTORY OF FILES ]**
  - `MainPage.xaml.cs`, `MapPage.xaml.cs`, `NewRegistrationPage.xaml.cs`, `DamageReportPage.xaml.cs`, `ChatPage.xaml.cs`.
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Infrastructure:** Create a `ViewModels` folder and implement a `BaseViewModel` (implementing `INotifyPropertyChanged`).
  - [ ] **Refactoring:** Extract all HTTP calls, state management, and navigation logic from code-behind files into dedicated ViewModels (e.g., `MainViewModel`, `MapViewModel`).
  - [ ] **Data Binding:** Replace procedural UI updates (e.g., `ParcelIdLabel.Text = ...`) with XAML Data Binding.
  - [ ] **Commanding:** Replace event handlers (e.g., `OnSaveClicked`) with `ICommand` implementations in ViewModels.

## 4. Hardware & Local Persistence
- **[ COMPLIANCE STATUS ]** Partial - Requires Rework
- **[ INVENTORY OF FILES ]**
  - **GPS:** `NewRegistrationPage.xaml.cs`, `MapPage.xaml.cs` (Implemented using `Geolocation`).
  - **Camera:** `NewRegistrationPage.xaml.cs` (Implemented using `MediaPicker`).
  - **Maps:** `MapPage.xaml` (Implemented via `WebView` + custom JS).
  - **SQLite:** **Missing**.
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Persistence:** Integrate `sqlite-net-pcl` to allow offline registration of wildlife observations.
  - [ ] **Hardware Logic:** Implement proper permission checking and error handling for GPS/Camera in a decoupled service.
  - [ ] **Spatial Logic:** Refactor the 300m safety radius calculation from JavaScript into a testable C# domain service.

## 5. Backend, Security & Cloud Matrix
- **[ COMPLIANCE STATUS ]** Partial - Requires Rework
- **[ INVENTORY OF FILES ]**
  - **Persistence:** `FaunaDbContext.cs` (EF Core with SQL Server).
  - **Security:** `UsersController.cs` (Manual role checking).
  - **JWT:** **Missing**.
  - **Proxy:** `ParcelsController.cs` (Integration with PDOK/Kadaster API).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Authentication:** Implement JWT Bearer Token authentication in the API.
  - [ ] **Authorization:** Apply `[Authorize(Roles = "...")]` attributes to all protected endpoints.
  - [ ] **Password Security:** Replace plain-text password storage with a secure hashing algorithm (e.g., BCrypt).
  - [ ] **User Service:** Update the MAUI `UserService` to handle token storage and inclusion in HTTP headers.

## 6. Agile Framework Alignment
- **[ COMPLIANCE STATUS ]** Missing Entirely
- **[ INVENTORY OF FILES ]**
  - `README.md` (Minimal technical overview).
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Documentation:** Create `UserStories.md` mapping features to exam requirements.
  - [ ] **Governance:** Define a "Definition of Done" (DoD) in `OPLEVERING.md` or a new `DOD.md`.
  - [ ] **Process:** Add metadata to methods/classes linking them to specific User Stories (e.g., `[UserStory("US-01")]`).

## 7. Verification Engineering (Testing Suite)
- **[ COMPLIANCE STATUS ]** Partial - Requires Rework
- **[ INVENTORY OF FILES ]**
  - **Unit Tests:** `FaunaConnect2.Api/Tests/` (Contains 4 tests).
  - **UI Tests:** **Missing**.
- **[ EXAM-GAP ACTION ITEMS ]**
  - [ ] **Unit Testing:** Increase coverage for complex business rules (e.g., safety radius, area overlap logic).
  - [ ] **UI Testing:** Implement at least 3 Appium or MAUI UI tests verifying the "Happy Path" for Hunter and Farmer roles.
  - [ ] **Acceptance:** Link tests to User Stories to prove DoD compliance.

---
**Audit Summary:** The project has a solid .NET 10 foundation but suffers from high coupling (Lack of MVVM), critical security gaps (No JWT/RBAC enforcement), and internationalization issues (Dutch leaks).
