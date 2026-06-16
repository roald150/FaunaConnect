# FaunaConnect 2.0 - App

De mobiele/desktop applicatie voor jagers en boeren.

## Technologieën
- **.NET MAUI**: Cross-platform UI framework.
- **Leaflet.js**: Interactieve kaart via een WebView.
- **HttpClient**: Communicatie met de backend.

## Kaart Integratie
De kaart bevindt zich in `MapPage`. Deze laadt `map.html` uit de `Resources/Raw` folder. Communicatie tussen C# en de kaart verloopt via `EvaluateJavaScriptAsync` en `invoke://` URL's.

## Configuratie
De API URL wordt geconfigureerd in `Services/UserService.cs`.
- Windows: `http://127.0.0.1:5282/api/`
- Android: `http://10.0.2.2:5282/api/`
