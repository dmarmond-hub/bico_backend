FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o código e compilar
COPY . ./
RUN dotnet publish Bico.csproj -c Release -o out

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