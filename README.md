# ToDo Application

Simple todo-list app built with asp.net core and angular

## Running the application

### Environment

```bash
    cd {project root}
    docker-compose -f docker-compose.yaml up -d
```

### Backend

```bash
    cd {project root}/src/ToDo.WebApi
    dotnet build ./ToDo.WebApi.csproj
    dotnet run ./ToDo.WebApi.csproj
```

### Frontend

```bash
    cd {project root}/src/ToDo.Client
    npm install
    ng serve
```

## Default URLs

- Swagger: <http://localhost:5000/swagger/index.html>
- API: <http://localhost:5000/api/{conroller}>
- SignalR Hub: <http://localhost:5000/todo>
- Client: <http://localhost:4200>
- pgAdmin: <http://localhost:5050>
  - username: admin
  - password: admin
- seq: <http://localhost:5341>

## Running ToDo Tests

To run tests in `ToDoTests.cs` first it is necessary to run the project for example with `CTRL+F5` in vs.
