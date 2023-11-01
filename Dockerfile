FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# First, copy the .csproj files and restore any dependencies (via dotnet restore).
COPY ./StudyBuddy/StudyBuddy.csproj ./StudyBuddy/
COPY ./Tests/StudyBuddy.Tests/StudyBuddy.Tests.csproj ./Tests/StudyBuddy.Tests/
RUN dotnet restore ./StudyBuddy/StudyBuddy.csproj
RUN dotnet restore ./Tests/StudyBuddy.Tests/StudyBuddy.Tests.csproj

# Then copy the whole application and publish.
COPY ./StudyBuddy/ ./StudyBuddy/
RUN dotnet publish ./StudyBuddy/StudyBuddy.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "StudyBuddy.dll"]
