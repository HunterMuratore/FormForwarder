#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FormForwarder/FormForwarder.csproj", "./FormForwarder/"]
COPY ["FormForwarder.sln", "./FormForwarder/"]
RUN dotnet restore "FormForwarder/FormForwarder.csproj"
COPY . .
WORKDIR "/src/FormForwarder"
RUN dotnet build "FormForwarder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FormForwarder.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FormForwarder.dll"]