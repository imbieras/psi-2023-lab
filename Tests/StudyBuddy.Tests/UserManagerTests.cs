using StudyBuddy.Managers;
using StudyBuddy.ValueObjects;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using System.Reflection;
namespace StudyBuddyTests
{
    public class UserManagerTests
    {
        [Fact]
        public void RegisterUser_ShouldCreateUserWithGeneratedUserId()
        {
            // Arrange
            var userManager = new UserManager();

            // Create a UserId instance using reflection
            Type userIdType = typeof(UserId);
            ConstructorInfo? userIdConstructor = userIdType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Guid) }, null);
            //var userIdInstance = (UserId)userIdConstructor.Invoke(new object[] { Guid.NewGuid() });

            if (userIdConstructor != null)
            {
                var userIdInstance = (UserId)userIdConstructor.Invoke(new object[] { Guid.NewGuid() });

                string username = "testUser";
                UserFlags flags = UserFlags.Registered;
                UserTraits traits = new UserTraits
                {
                    Birthdate = DateTime.Now,
                    Subject = "Math",
                    AvatarPath = "/avatars/testUser.jpg",
                    Description = "A test user",
                    Hobbies = new System.Collections.Generic.List<string> { "Reading", "Gaming" }
                };

                // Act
                UserId registeredUserId = userManager.RegisterUser(username, flags, traits);

                // Assert
                IUser registeredUser = userManager.GetUserById(registeredUserId)!;
                Assert.NotNull(registeredUser);
                Assert.Equal(username, registeredUser.Name);
                Assert.Equal(flags, registeredUser.Flags);
                Assert.Equal(traits.Birthdate, registeredUser.Traits.Birthdate);
                Assert.Equal(traits.Subject, registeredUser.Traits.Subject);
                Assert.Equal(traits.AvatarPath, registeredUser.Traits.AvatarPath);
                Assert.Equal(traits.Description, registeredUser.Traits.Description);
                Assert.Equal(traits.Hobbies, registeredUser.Traits.Hobbies);
            }
        }

        [Fact]
        public void GetUserByUsername_ShouldReturnUser()
        {
            // Arrange
            var userManager = new UserManager();
            string username = "testUser";
            UserFlags flags = UserFlags.Registered;
            UserTraits traits = new UserTraits();

            // Act
            UserId registeredUserId = userManager.RegisterUser(username, flags, traits);
            IUser user = userManager.GetUserByUsername(username)!;

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username, user.Name);
            Assert.Equal(flags, user.Flags);
        }

        [Fact]
        public void BanUser_ShouldAddBannedFlag()
        {
            // Arrange
            var userManager = new UserManager();
            string username = "testUser";
            UserFlags flags = UserFlags.Registered;
            UserTraits traits = new UserTraits();

            // Act
            UserId registeredUserId = userManager.RegisterUser(username, flags, traits);
            userManager.BanUser(registeredUserId);
            IUser bannedUser = userManager.GetUserById(registeredUserId)!; // Use ! to suppress the warning

            // Assert
            Assert.NotNull(bannedUser);
            Assert.True(bannedUser.Flags.HasFlag(UserFlags.Banned));
        }

        [Fact]
        public static void GetAllUsers_ShouldReturnAllRegisteredUsers()
        {
            // Arrange
            var userManager = new UserManager();
            string username1 = "user1";
            string username2 = "user2";
            string username3 = "user3";

            // Act
            UserId userId1 = userManager.RegisterUser(username1, UserFlags.Registered, new UserTraits());
            UserId userId2 = userManager.RegisterUser(username2, UserFlags.Registered, new UserTraits());
            UserId userId3 = userManager.RegisterUser(username3, UserFlags.Registered, new UserTraits());
            List<IUser> allUsers = userManager.GetAllUsers();

            // Assert
            Assert.NotNull(allUsers);
            Assert.Equal(3, allUsers.Count);
            Assert.Contains(allUsers, user => user.Name == username1);
            Assert.Contains(allUsers, user => user.Name == username2);
            Assert.Contains(allUsers, user => user.Name == username3);
        }

        [Fact]

        public void GetRandomUser_ShouldReturnValidUser()
        {
            // Arrange
            var userManager = new UserManager();
            string username1 = "user1";
            string username2 = "user2";
            string username3 = "user3";

            // Act
            UserId userId1 = userManager.RegisterUser(username1, UserFlags.Registered, new UserTraits());
            UserId userId2 = userManager.RegisterUser(username2, UserFlags.Registered, new UserTraits());
            UserId userId3 = userManager.RegisterUser(username3, UserFlags.Registered, new UserTraits());

            // Get the user
            IUser user1 = userManager.GetUserById(userId1)!;
            IUser user2 = userManager.GetUserById(userId2)!;
            IUser user3 = userManager.GetUserById(userId3)!;

            // Ensure the user is not null before calling GetRandomUser
            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.NotNull(user3);

            // Retrieve a random user
            IUser randomUser = userManager.GetRandomUser(user1)!;

            // Assert
            Assert.NotNull(randomUser);
            Assert.NotEqual(userId1, randomUser.Id); // Ensure it's not the same as the current user
            Assert.Contains(randomUser, userManager.GetAllUsers()); // Ensure it's a valid user in the system


            randomUser = userManager.GetRandomUser(user2)!;
            Assert.NotNull(randomUser);
            Assert.NotEqual(userId3, randomUser.Id); // Ensure it's not the same as the current user
            Assert.Contains(randomUser, userManager.GetAllUsers());// Ensure it's a valid user in the system


            randomUser = userManager.GetRandomUser(user3)!;
            Assert.NotNull(randomUser);
            Assert.NotEqual(userId3, randomUser.Id); // Ensure it's not the same as the current user
            Assert.Contains(randomUser, userManager.GetAllUsers());// Ensure it's a valid user in the system
       
        }
    }
}
