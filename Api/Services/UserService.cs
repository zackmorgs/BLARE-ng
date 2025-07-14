using BCrypt.Net;
using Models;
using MongoDB.Driver;

namespace Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        // constructor that
        // 1. intializes MongoDB client with ./appSettings.json "connectionString.MongoConnection"
        // 2. gets the "blare" database
        // 3. intializes the _users collection
        public UserService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
            var database = client.GetDatabase("blare");
            // gets all users in collection via "users" collection name
            _users = database.GetCollection<User>("users");
        }

        // interates through _users collection to find a the first user with that username
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        // iterates throguh _users collection to find the first user with that email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(
            string username,
            string email,
            string password,
            string role
        )
        {
            // only one admin account right now..
            if (role == "listener" || role == "artist")
            {
                if (await GetUserByUsernameAsync(username) == null)
                {
                    var user = new User
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        Avatar = String.Empty,
                        Role = role,
                    };

                    await _users.InsertOneAsync(user);
                    return user;
                }
                else
                {
                    throw new ArgumentException("Username already exists.");
                }
            }
            else
            {
                throw new ArgumentException(
                    "Invalid role specified. Allowed roles are 'listener' or 'artist'."
                );
            }
        }

        // Verifies the password against the stored hash
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        // gets the user from GetUserByUsernameAsync method
        // checks if user is active, not null, and verifies the password is correct
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null || !user.IsActive || !VerifyPassword(password, user.PasswordHash))
            {
                // returns null if user is not found, is inactive, or password does not match
                return null;

            }
            return user;
        }

        // test this
        public async Task<bool> UpdateUserAvatarAsync(string userId, string avatarUrl)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Avatar, avatarUrl);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // updates the user's role

        public async Task<bool> UpdateUserRoleAsync(string userId, string role)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Role, role);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }
    }
}
