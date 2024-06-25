using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmt.Core;
using UserMgmt.Core.Models;

namespace UserMgmt.API
{
    public class UserRepository : IUserRepository
    {
        private readonly UserMgmtDbContext _context;

        public UserRepository(UserMgmtDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            try
            {

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error saving user: " + ex.Message);
                throw;
             }
            
        }

        public async Task<User> UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Retrieve the existing user from the context
            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.UserId} not found.");
            }

            if (existingUser != null)
            {
                // Update existingUser with new data
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.Alias = user.Alias;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
            }

                try
            {
                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency exceptions if needed
                throw new Exception("Concurrency error occurred while updating user.", ex);
            }

        }

        public async Task<bool> DeleteAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error deleting user: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return Enumerable.Empty<User>(); // Return empty if no search term
                }

                var searchTermLower = searchTerm.ToLower();
                var users = await _context.Users
                    .Where(u => u.FirstName.ToLower().Contains(searchTermLower) ||
                                  u.LastName.ToLower().Contains(searchTermLower) ||
                                  u.Email.ToLower().Contains(searchTermLower))
                    .ToListAsync();

                return users;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error searching user: " + ex.Message);
                throw;
            }
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error finding user: " + ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error finding all users: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByTypeAsync(string userType)
        {
            try
            {
                return await _context.Users.Where(u => u.UserType == userType).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error finding  users by usertype: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> AssignManagerAsync(int clientId, int managerId)
        {
            var client = await GetByIdAsync(clientId);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            var manager = await GetByIdAsync(managerId);
            if (manager == null)
            {
                throw new Exception("Manager not found");
            }

            if (ClientHasManager(clientId))
            {
                throw new Exception("Client already has a manager assigned.");
            }

            var relationship = new UserRelationship { ClientId = clientId, ManagerId = managerId };
            _context.UserRelationships.Add(relationship);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Failed to assign manager: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetManagersWithClientsAsync()
        {
            return await _context.Users
                            .Include(u => u.ClientRelationships) 
                            .Where(u => u.UserType == "Manager")
                            .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetClientsWithManagersAsync()
        {
            return await _context.Users
                                       .Include(u => u.ManagerRelationships)
                                       .Where(u => u.UserType == "Client")
                                       .ToListAsync();

        }

        public async Task<IEnumerable<User>> GetClientsForManagerAsync(string username)
        {
            return await _context.Users
                              .Include(u => u.ClientRelationships)
                              .Where(u => u.UserName == username)
                              .ToListAsync();

        }

        public async Task<bool> ReassignClientManagerAsync(int clientId, int newManagerId)
        {
            try
            {
                var client = await GetByIdAsync(clientId);
                if (client == null)
                {
                    throw new Exception("Client not found");
                }

                if (!ClientHasManager(clientId))
                {
                    throw new Exception("Client does not have a manager assigned.");
                }

                var existingRelationship = await _context.UserRelationships
                    .FirstOrDefaultAsync(r => r.ClientId == clientId);
                if (existingRelationship != null)
                {
                    existingRelationship.ManagerId = newManagerId;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Failed to re-assign manager: {ex.Message}");
            }
        }

        private bool ClientHasManager(int clientId)
        {
            return _context.UserRelationships.Any(r => r.ClientId == clientId);
        }
        

    }

}
