# Oplevering FaunaConnect 2.0

Dit document dient als naslagwerk voor de oplevering van de FaunaConnect 2.0 applicatie. Hierin worden de architectuur, functionaliteiten en technische keuzes toegelicht.

## 1. Project Overzicht
FaunaConnect 2.0 is een platform ontworpen voor jagers en boeren om wildwaarnemingen en schademeldingen te beheren. Het systeem bestaat uit twee hoofdonderdelen:
- **FaunaConnect2.App**: Een mobiele/desktop applicatie gebouwd met .NET MAUI.
- **FaunaConnect2.Api**: Een backend REST-API gebouwd met ASP.NET Core en Entity Framework Core.

## 2. Belangrijkste Functionaliteiten

### Interactieve Kaart (MapPage)
- Gebruikt **Leaflet.js** via een WebView voor een soepele kaart-ervaring.
- **Perceelselectie**: Gebruikers kunnen percelen aanklikken op de kaart. De gegevens (zoals grootte in hectare) worden real-time opgehaald uit de API.
- **BBOX Inladen**: Om de kaart snel te houden, worden alleen percelen ingeladen die zichtbaar zijn op het scherm (Bounding Box). Dit gebeurt pas vanaf zoomniveau 16 ("alles of niets" principe). Wanneer je verder uitzoomt, worden de percelen automatisch verwijderd om de kaart overzichtelijk te houden.
- **Laden Indicator**: Tijdens het ophalen van percelen wordt een spinner getoond om de gebruiker feedback te geven.
- **Klik Precisie**: Er is een `Point-in-Polygon` algoritme geïmplementeerd. Dit zorgt ervoor dat wanneer je klikt, exact het juiste perceel geselecteerd wordt, zelfs als er meerdere percelen in de buurt liggen.
- **Jachtvelden**: Jagers kunnen geselecteerde percelen opslaan als hun eigen jachtveld.

### Dashboard (MainPage)
- **Weer & Zon**: Toont actuele windinformatie en zonsopkomst/-ondergang tijden.
- **Overzicht**: Een snelle lijst van de laatste wildwaarnemingen.

### Waarnemingen & Schade
- Gebruikers kunnen nieuwe waarnemingen (ree, wild zwijn, etc.) registreren.
- Boeren en jagers kunnen wildschade melden op specifieke locaties.

## 3. Technische Realisatie

### Communicatie App naar API
- De App communiceert met de API via `HttpClient`. 
- Voor Android wordt het adres `10.0.2.2` gebruikt (de loopback naar de host machine).
- Voor Windows wordt `127.0.0.1` gebruikt.

### WebView naar C# Communicatie
- In de kaart wordt communicatie van JavaScript naar C# gedaan via een aangepast URL-schema: `invoke://mapclick?lat=...&lng=...`. 
- De `OnWebViewNavigating` event-handler in C# onderschept deze verzoeken en voert de bijbehorende logica uit.

### Database
- Er wordt gebruik gemaakt van **SQL Server (LocalDB)** met **Entity Framework Core**.
- Bij het opstarten van de API wordt de database automatisch aangemaakt (`EnsureCreated`) en gevuld met testdata (Seed Data) als deze nog niet bestaat.

## 4. Testen
Er zijn 3 tests toegevoegd om de kernfunctionaliteit te garanderen:
1. **WeatherControllerTest**: Controleert of de API weer-informatie correct teruggeeft.
2. **AnimalSpeciesControllerTest**: Valideert het ophalen van de lijst met beschikbare diersoorten.
3. **WindDirectionLogicTest**: Test de C# logica die graden omzet naar windrichtingen (bijv. 240° -> ZW).

## 5. Veelgestelde Vragen (voor Presentatie)

**V: Hoe werkt de kaart offline?**
A: De kaart gebruikt momenteel online tiles van OpenStreetMap/CartoDB. Voor volledige offline werking zouden mbtiles moeten worden toegevoegd.

**V: Hoe worden de percelen opgehaald?**
A: Zodra de gebruiker de kaart verschuift of inzoomt, wordt de 'Bounding Box' (coördinaten van de hoeken van het scherm) naar de API gestuurd. De API filtert de database op alle percelen die binnen deze box vallen.

**V: Waarom is er gekozen voor een WebView voor de kaart?**
A: Hiermee hebben we volledige controle over de visualisatie van complexe polygonen (percelen), wat met de standaard MAUI Maps beperkter is.
