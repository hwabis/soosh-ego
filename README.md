## Soosh Ego 😵‍💫

This is an online implementation of the [Sushi Go!](https://boardgamegeek.com/boardgame/133473/sushi-go) card game.

## Development

### Server

From the server root directory:
```
cp .env.example .env
```

Run the project with one of the following:
- Run the docker-compose project in an IDE such as Visual Studio
- `docker compose up`

View the database content with one of the following:
- Terminal
  - Docker exec into the db container, either through a terminal or Docker Desktop
  - ```
    bash
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -No
    ```
- Azure Data Studio
  - Server in the connection string would be `localhost`

### Client

From the client root directory:
```
cp .env.example .env
npm install
npm run dev
```
