# FaunaConnect2 - Project Documentatie

Welkom bij de documentatie van **FaunaConnect2**. Dit document legt uit hoe het systeem is opgebouwd, welke technologieën er zijn gebruikt en waarom er voor bepaalde oplossingen is gekozen.

---

## 1. Inleiding
FaunaConnect2 is een platform ontworpen voor jagers en landbouwers om wildregistraties, schademeldingen en communicatie te stroomlijnen. Het systeem bestaat uit een **Backend (Web API)** en een **Frontend (Mobiele App)**.

---

## 2. Architectuur Overzicht
Het project volgt een **Client-Server architectuur**:
- **Backend**: ASP.NET Core Web API die de data beheert en serveert.
- **Frontend**: .NET MAUI applicatie voor cross-platform gebruik (Android, iOS, Windows).

---

## 3. Backend (FaunaConnect2.Api)

De backend is gebouwd met **ASP.NET Core 10**.

### Technologieën & Keuzes
- **Entity Framework Core (EF Core)**: Gebruikt als ORM (Object-Relational Mapper). Dit maakt het mogelijk om met de database te communiceren via C# objecten in plaats van ruwe SQL-queries.
- **SQLite**: Een lichtgewicht database die in een bestand wordt opgeslagen.
    - *Waarom?* Ideaal voor ontwikkeling en demo's omdat er geen aparte database-server (zoals SQL Server) geïnstalleerd hoeft te worden.
- **JWT (JSON Web Tokens)**: Voor beveiliging en authenticatie.
    - *Waarom?* Het is een standaard voor stateless authenticatie in API's. De server hoeft geen sessies te onthouden, wat de schaalbaarheid ten goede komt.
- **BCrypt.Net**: Voor het veilig hashen van wachtwoorden. Wachtwoorden worden nooit in tekstvorm opgeslagen.
- **Swagger/OpenAPI**: Automatisch gegenereerde documentatie voor de API-endpoints.

### Kernfunctionaliteiten
- **Controllers**: Behandelen HTTP-verzoeken (GET, POST, etc.) voor Users, Registrations, Chat, Weather, etc.
- **Weather Integration**: Haalt zonsopgang- en zonsondergangtijden op via een externe API (`sunrise-sunset.org`) en gebruikt mock-data voor actuele weersomstandigheden (om API-kosten te vermijden).

---

## 4. Frontend (FaunaConnect2.App)

De app is gebouwd met **.NET MAUI**.

### MVVM Patroon
De app volgt strikt het **Model-View-ViewModel (MVVM)** patroon:
- **Models**: De data-structuren (bijv. `Registration`, `User`).
- **Views**: De UI (XAML bestanden) zoals `MainPage.xaml`.
- **ViewModels**: De logica achter de schermen die de data voorbereidt voor de View.
    - *Waarom?* Dit zorgt voor een schone scheiding tussen de interface en de logica, wat het testen en onderhouden makkelijker maakt.

### Belangrijke Services
- **SpatialService**: Bevat complexe algoritmen zoals *Ray Casting* om te controleren of een coördinaat binnen een jachtveld valt, en de *Haversine formule* voor afstandsberekeningen.
- **UserService**: Beheert de login-status en communicatie met de API voor gebruikersbeheer.
- **LocalDatabaseService**: Gebruikt SQLite op het apparaat voor offline opslag of caching.
- **DeviceService**: Handelt apparaat-specifieke zaken af zoals locatievoorzieningen.

---

## 5. Belangrijke Features

### 📍 Kaart & Locatie
De app maakt gebruik van interactieve kaarten. Gebruikers kunnen hun locatie pinnen voor een nieuwe registratie. Er is een `LocationPickerPage` die gebruik maakt van een WebView met Leaflet voor nauwkeurige selectie.

### 🦌 Wildregistraties
Jagers kunnen geschoten wild of waarnemingen registreren. Elke registratie bevat coördinaten, diersoort en de jager die de registratie deed.

### 💬 Chat & Notificaties
Er is een ingebouwd chatsysteem voor communicatie tussen jagers en landbouwers. Notificaties houden gebruikers op de hoogte van nieuwe schademeldingen of berichten.

### 🌦️ Weer & Jachtijden
Omdat jachtijden vaak gebonden zijn aan zonsopgang en -ondergang, haalt de app deze data op basis van de huidige locatie van de gebruiker.

---

## 6. Waarom deze keuzes? (De "Standaard")

1. **Dependency Injection (DI)**: In zowel de API als de App gebruiken we DI.
    - *Waarom?* Het maakt componenten losgekoppeld (loosely coupled). In plaats van dat een klasse zelf zijn afhankelijkheden aanmaakt, krijgt hij deze "geïnjecteerd". Dit maakt het systeem flexibel en makkelijk testbaar.
2. **Asynchrone Programmering (async/await)**:
    - *Waarom?* Om de app responsief te houden en de API niet te blokkeren tijdens zware taken (zoals database-acties of netwerkverzoeken).
3. **Zure (Surgical) Updates**: In de code gebruiken we vaak specifieke updates in plaats van hele objecten te overschrijven, wat bandbreedte en database-resources bespaart.

---

## 7. Hoe te starten
1. Open de solution (`FaunaConnect2.sln`) in Visual Studio of JetBrains Rider.
2. Start eerst de **FaunaConnect2.Api** (zorg dat de database wordt gegenereerd bij de eerste run).
3. Start daarna de **FaunaConnect2.App** op een simulator of fysiek apparaat.

*Gemaakt door Gemini CLI - Juni 2026*
