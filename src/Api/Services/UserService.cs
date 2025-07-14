using BCrypt.Net;
using Models;
using MongoDB.Driver;
using Data;

namespace Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(DataContext dataContext)
        {
            _users = dataContext.Users;
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

        public async Task<bool> UpdateUserInfoAsync(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                throw new ArgumentException("User cannot be null and must have a valid Id.");
            }
            else
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                var update = Builders<User>
                    .Update
                    .Set(u => u.FirstName, user.FirstName)
                    .Set(u => u.LastName, user.LastName)
                    .Set(u => u.Bio, user.Bio);

                var result = await _users.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, Controllers.UpdateProfileRequest request)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var updateBuilder = Builders<User>.Update;
            var updates = new List<UpdateDefinition<User>>();

            if (!string.IsNullOrEmpty(request.FirstName))
                updates.Add(updateBuilder.Set(u => u.FirstName, request.FirstName));

            if (!string.IsNullOrEmpty(request.LastName))
                updates.Add(updateBuilder.Set(u => u.LastName, request.LastName));

            if (!string.IsNullOrEmpty(request.Bio))
                updates.Add(updateBuilder.Set(u => u.Bio, request.Bio));

            if (!string.IsNullOrEmpty(request.Avatar))
                updates.Add(updateBuilder.Set(u => u.Avatar, request.Avatar));

            if (updates.Count == 0)
                return false;

            var combinedUpdate = updateBuilder.Combine(updates);
            var result = await _users.UpdateOneAsync(filter, combinedUpdate);
            return result.ModifiedCount > 0;
        }

        // Set a specific user as admin (call this once to create your admin)
        public async Task<bool> SetUserAsAdmin(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return false;

            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update.Set(u => u.Role, "admin");
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, string role)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Role, role);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
