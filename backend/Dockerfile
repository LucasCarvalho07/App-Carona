# Imagem de desenvolvimento: SDK completo + dotnet watch (hot reload).
FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src
COPY . .
RUN dotnet restore AppCarona.slnx

WORKDIR /src/src/AppCarona.Api

# Polling é mais confiável para file-watch dentro de container.
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

CMD ["dotnet", "watch", "run", "--no-launch-profile"]
