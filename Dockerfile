# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["Mecanica.Hermes.sln", "./"]
COPY ["src/", "src/"]

RUN dotnet restore "src/Mecanica.Hermes.Api/Mecanica.Hermes.Api.csproj"

# Remover binários antigos para garantir um build limpo
RUN find /src/src -type d -name bin -prune -exec rm -rf {} +

# Publish da aplicação
WORKDIR "/src/src/Mecanica.Hermes.Api"
FROM build AS publish
RUN dotnet publish "Mecanica.Hermes.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final - runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Instalar curl para healthcheck e criar usuário não-root para segurança
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/* && \
    groupadd --system --gid 1001 appuser && \
    useradd --system --uid 1001 --gid appuser --no-create-home appuser

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Alterar proprietário dos arquivos
RUN chown -R appuser:appuser /app

# Usar usuário não-root
USER appuser

# Expor porta
EXPOSE 8080

# Variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Healthcheck simples na porta 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

# Comando de inicialização
ENTRYPOINT ["dotnet", "Mecanica.Hermes.Api.dll"]
