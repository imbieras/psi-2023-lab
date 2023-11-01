# StudyBuddy

## Running the Project with Docker

Before running the project with Docker, make sure you have Docker and Docker Compose installed on your system. If not, you can install them following the official Docker documentation: [Install Docker](https://docs.docker.com/get-docker/).

### Initial Database Setup

In the "StudyBuddy" directory, run the following commands to set up the initial database:

```bash
$ cd StudyBuddy
$ dotnet ef migrations add <MigrationName>
$ dotnet ef database update
```

This will create the necessary database schema.

### Create an Environment Configuration File

Before running the project with Docker, create an environment configuration file (`.env`) in the project root directory. This file will contain sensitive information, such as your connection string and PostgreSQL variables. Here's an example of what your `.env` file might look like:

```
# .env
ASPNETCORE_URLS=http://*:80
ConnectionStrings__DefaultConnection=Host=db;Database=studybuddy;Username=studybuddy;Password=studybuddypassword
POSTGRES_USER=studybuddy
POSTGRES_PASSWORD=studybuddypassword
POSTGRES_DB=studybuddy
```

Make sure to replace the values in the connection string and variables with your actual database and authentication details.

### Running the Project

#### Without Hot Reloading

If you don't need hot reloading and are fine with rebuilding the Docker container for each change:

1. Build the Docker containers for the project:

```bash
$ docker-compose build
```

You can also choose to rebuild only the web or database container by running `docker-compose build web` or `docker-compose build db`, respectively.

2. Start the project:

```bash
$ docker-compose up
```

Use `docker-compose up -d` to run the project in a detached state.

3. Access the project in your browser by navigating to http://localhost:8000.

4. To stop the project, use `CTRL-C`, or if you ran it detached, use:

```bash
$ docker-compose down
```

Repeat these steps for any code changes you make.

#### With Hot Reloading

For a more efficient development process with hot reloading:

1. Build the Docker containers for the project:

```bash
$ docker-compose build
```

2. Start the project in the first terminal:

```bash
$ docker-compose up
```

3. Open a second terminal and run the following command to enable hot reloading:

``` bash
$ dotnet watch
```

This will automatically update the project when code changes occur.

4. The project should open in your browser, and changes will be reflected without manual rebuilding. (Note that for some changes, you may still need to rebuild the Docker containers as necessary.)

5. To stop the project, use `CTRL-C` on both terminals, then rebuild if needed.

## Functional requirements

### MVP

1. Profile Creation
2. Very Basic User Authentication
3. Matchmaking
4. In-app Messaging
5. Swiping Mechanism
6. Basic Search and Filters
7. User Preferences (age, distance)

### Nice to have

1. Reporting, Blocking and Banning
2. Study Group Creation
3. File and Image Sharing in Messaging
4. Meeting time planning utility

## User flow

1. Register/Sign In
2. Create Profile
3. Set Preferences
4. Navigate to Home Feed
5. Swipe Profiles
6. Match Confirmation
7. Start Messaging
8. Plan a meeting time
