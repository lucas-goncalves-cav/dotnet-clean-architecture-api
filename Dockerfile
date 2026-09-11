FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY *.sln ./
COPY src/Domain/*.csproj src/Domain/
COPY src/Application/*.csproj src/Application/
COPY src/Infrastructure/*.csproj src/Infrastructure/
COPY src/WebApi/*.csproj src/WebApi/
COPY tests/UnitTests/*.csproj tests/UnitTests/
COPY tests/IntegrationTests/*.csproj tests/IntegrationTests/
RUN dotnet restore

COPY . .
RUN dotnet publish src/WebApi/CleanArchitecture.WebApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "CleanArchitecture.WebApi.dll"]
