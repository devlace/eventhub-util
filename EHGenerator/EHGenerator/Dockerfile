FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["EHGenerator/EHGenerator.csproj", "EHGenerator/"]
RUN dotnet restore "EHGenerator/EHGenerator.csproj"
COPY . .
WORKDIR "/src/EHGenerator"
RUN dotnet build "EHGenerator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "EHGenerator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EHGenerator.dll"]