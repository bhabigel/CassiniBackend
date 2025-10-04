ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${DOTNET_RUNTIME} AS base
WORKDIR /app
EXPOSE 80 443

FROM ${DOTNET_SDK} AS build
WORKDIR /src

COPY ["CassiniConnect.API/CassiniConnect.API.csproj", "CassiniConnect.API/"]
COPY ["CassiniConnect.Application/CassiniConnect.Application.csproj", "CassiniConnect.Application/"]
COPY ["CassiniConnect.Core/CassiniConnect.Core.csproj", "CassiniConnect.Core/"]

RUN dotnet restore "CassiniConnect.API/CassiniConnect.API.csproj"

COPY . .

WORKDIR /src/CassiniConnect.API
RUN dotnet publish "CassiniConnect.API.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet", "CassiniConnect.API.dll" ]
