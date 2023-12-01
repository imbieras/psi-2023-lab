# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy .csproj files and restore dependencies for StudyBuddy
COPY ./StudyBuddy/StudyBuddy.csproj ./StudyBuddy/
RUN dotnet restore ./StudyBuddy/StudyBuddy.csproj

# Copy libman.json and restore client-side libraries for StudyBuddy
COPY ./StudyBuddy/libman.json ./StudyBuddy/
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="${PATH}:/root/.dotnet/tools"
WORKDIR /app/StudyBuddy/
RUN libman restore

# Copy .csproj files and restore dependencies for StudyBuddy.API
COPY ./StudyBuddy.API/StudyBuddy.API.csproj ./StudyBuddy.API/
RUN dotnet restore ./StudyBuddy.API/StudyBuddy.API.csproj

# Copy the whole applications for StudyBuddy and StudyBuddy.API
COPY ./StudyBuddy/ ./StudyBuddy/
COPY ./StudyBuddy.API/ ./StudyBuddy.API/

# Publish StudyBuddy
WORKDIR /app/StudyBuddy/
RUN dotnet publish ./StudyBuddy.csproj -c Release -o out

# Publish StudyBuddy.API
WORKDIR /app/StudyBuddy.API/
RUN dotnet publish ./StudyBuddy.API.csproj -c Release -o out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy the published output from StudyBuddy
COPY --from=build-env /app/StudyBuddy/out ./StudyBuddy/

# Copy the published output from StudyBuddy.API
COPY --from=build-env /app/StudyBuddy.API/out ./StudyBuddy.API/

# Expose port 80 for StudyBuddy
EXPOSE 80

# Define the entry point for the container for StudyBuddy
ENTRYPOINT ["dotnet", "StudyBuddy/StudyBuddy.dll"]

# Additional entry point for StudyBuddy.API if needed
ENTRYPOINT ["dotnet", "StudyBuddy.API/StudyBuddy.API.dll"]
