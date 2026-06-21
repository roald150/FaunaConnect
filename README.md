# FaunaConnect2

Platform voor jagers en landbouwers om wildregistraties, schademeldingen en communicatie te stroomlijnen.

## Vereisten

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Voor de App: Visual Studio 2022+ of JetBrains Rider met MAUI-werkload

## Installatie & Gebruik

### 1. API starten

```bash
cd FaunaConnect2.Api
dotnet run
```

De API draait op `http://localhost:5282`. Bij de eerste start wordt een SQLite-database (`FaunaConnect2.db`) aangemaakt en gevuld met testdata. Swagger-UI is beschikbaar op `/swagger`.

### 2. App starten

```bash
cd FaunaConnect2.App
dotnet run --framework net10.0-windows10.0.19041.0
```
Of open `FaunaConnect2.sln` in Visual Studio/Rider en start `FaunaConnect2.App`.

### Testaccounts

| Email             | Wachtwoord | Rol     |
|-------------------|------------|---------|
| roald@jacht.nl    | welcome    | Jager   |
| harms@boerderij.nl| welcome    | Boer    |
| admin@jachtveld.nl| admin      | Admin   |

### Korte functionaliteiten

- **Wildregistraties**: voeg locatie en diersoort toe via de kaart
- **Schademeldingen**: dien een schadeclaim in als boer
- **Chat**: communiceer tussen jagers en landbouwers
- **Jachtijden**: bekijk zonsopgang- en zonsondergang op basis van locatie
- **Admin**: beheer gebruikers en percelen
