# FaunaConnect 2.0 - API

Deze API vormt de backbone van het FaunaConnect platform.

## Architectuur
- **ASP.NET Core 10.0**: Web framework.
- **Entity Framework Core**: ORM voor database interactie.
- **SQL Server**: Persistente opslag.

## Belangrijkste Endpoints
- `/api/parcels`: Ophalen van perceelgegevens.
- `/api/huntinggrounds`: Beheer van jachtgebieden.
- `/api/weather`: Actuele weerinformatie (mock).
- `/api/sun`: Zonsopkomst en -ondergang informatie.

## Database Initialisatie
Bij het opstarten controleert de API of de database bestaat. Zo niet, dan wordt deze aangemaakt en gevuld met basisgegevens (Jagers, Boeren, Diersoorten) in `Program.cs`.
