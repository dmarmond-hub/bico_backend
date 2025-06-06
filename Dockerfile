FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY Bico.csproj ./
RUN dotnet restore

# Copy only necessary files
COPY Controllers/ ./Controllers/
COPY Models/ ./Models/
COPY Services/ ./Services/
COPY Program.cs ./
COPY appsettings.json ./
COPY credentials.json ./

# Build the app
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Bico.dll"]