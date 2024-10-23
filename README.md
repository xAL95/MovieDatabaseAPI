# MovieDatabaseAPI

MovieDatabaseAPI is a sample ASP.NET Core API backend project for managing a simple movie database. It uses [LiteDB](https://www.litedb.org/) as the database to store, load, and edit movie data, and includes [Swagger](https://swagger.io/) for API documentation and testing. The project also features API-Key-based authentication for basic security.

## Features

- **CRUD Operations**: Create, read, update, and delete movies in the database.
- **LiteDB Integration**: Uses LiteDB, a lightweight, embedded NoSQL database, for data storage.
- **Swagger UI**: Provides an interactive interface for testing the API endpoints.
- **API-Key Authentication**: Secures the API using an API key mechanism.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- A code editor, such as [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/)

### Installation

1. **Clone the repository**
    ```bash
    git clone https://github.com/xAL95/MovieDatabaseAPI.git
    ```
2. **Navigate to the project folder**
    ```bash
    cd MovieDatabaseAPI
    ```
3. **Restore the dependencies**
    ```bash
    dotnet restore
    ```
4. **Run the application**
    ```bash
    dotnet run
    ```
5. **Access the Swagger UI**
   Open a web browser and navigate to `https://localhost:7081/index.html` (or the configured URL) to explore the API.

### API Key Configuration

To access the API, you need to provide a valid API key in the request headers. The default API key can be configured in the `appsettings.json` file:

```json
{
  "ApiKey": "your-secure-api-key"
}
