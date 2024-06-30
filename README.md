# BookLibrary

BookLibrary to aplikacja ASP.NET Core MVC do zarządzania biblioteką książek z funkcjami rejestracji i logowania użytkowników.

## Wymagania wstępne

Przed uruchomieniem projektu upewnij się, że masz zainstalowane następujące narzędzia:
- [Visual Studio 2022](https://visualstudio.microsoft.com/) lub nowszy
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) lub [LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

## Konfiguracja projektu

1. Sklonuj repozytorium:
   ```sh
   git clone https://github.com/TwojeKonto/BookLibrary.git
   cd BookLibrary

2. Otwórz projekt w Visual Studio
3. Przywróć zależności NuGet: 
	- Kliknij na rozwiązanie w solutnion Explorer i wybierz "Restore NuGet Packages" 
4. Migrację bazy danych: 
	- Otwórz Package Manager Console i wykonaj polecenia:
	a) Add-Migration InitialCreate
	b) Update-Database
	
## Uruchom aplikację w Visual Studio
