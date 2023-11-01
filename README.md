# StudyBuddy

# Running the project with docker:

- You must have docker and docker-compose installed
- `dotnet ef migrations add <MigrationName>` **in the StuddyBuddy directory** to add migrations
- Run `dotnet ef database update` **in the StuddyBuddy directory** to update the database

## Running without hot reloading (you will have to rebuild the docker container for every change)
1. `docker-compose build` or `docker-compose build web` / `docker-compose build db` if you need to rebuild only one part of the project
2. `docker-compose up` to run the project (`docker-compose up -d` to run in a detached state)
3. Go to `localhost:8000` on your browser
4. CTRL-C to shut down the docker containers (`docker-compose down` if you ran it detached)
5. Repeat the steps for any changes you make

## Running with hot reloading (I suggest you open two terminals)
1. `docker-compose build`
2. `docker-compose up` (first terminal)
3. `dotnet watch` (second terminal)
4. The project should automatically open and hot reload
5. For some changes you will still need to rebuild the docker containers
6. CTRL-C on both of the terminals will stop the project, then you can rebuild

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
8. Match Confirmation
9. Start Messaging
10. Plan a meeting time
