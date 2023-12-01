FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StudyBuddy/StudyBuddy.csproj", "StudyBuddy/"]
COPY ["StudyBuddy.Shared/StudyBuddy.Shared.csproj", "StudyBuddy.Shared/"]
RUN dotnet restore "StudyBuddy/StudyBuddy.csproj"
COPY . .
WORKDIR "/src/StudyBuddy"

# Add libman part
COPY ["StudyBuddy/libman.json", "StudyBuddy/"]
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN libman restore

RUN dotnet build "StudyBuddy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudyBuddy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudyBuddy.dll"]
