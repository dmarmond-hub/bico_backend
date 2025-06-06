FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar apenas os arquivos necessários
COPY Controllers/ ./Controllers/
COPY Models/ ./Models/
COPY Services/ ./Services/
COPY Properties/ ./Properties/
COPY wwwroot/ ./wwwroot/
COPY Program.cs ./
COPY appsettings*.json ./
COPY credentials*.json ./
COPY CertHomol.p12 ./

# Compilar o projeto
RUN dotnet publish -c Release -o out

# Build da imagem de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
COPY --from=build /app/credentials.json ./
COPY --from=build /app/CertHomol.p12 ./

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expor a porta que o app vai usar
EXPOSE 8080

# Iniciar a aplicação
ENTRYPOINT ["dotnet", "Bico.dll"]