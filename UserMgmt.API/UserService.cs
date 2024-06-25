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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserMgmtDbContext _context;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return await _userRepository.AddAsync(user); ;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }


            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (userId == 0)
            {
                throw new ArgumentNullException();
            }


            return await _userRepository.DeleteAsync(userId);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (userId == 0)
            {
                throw new ArgumentNullException();
            }


            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Enumerable.Empty<User>(); // Return empty if no search term
            }

            var users = await _userRepository.SearchUsersAsync(searchTerm);
            return users;
        }

        public async Task<IEnumerable<User>> GetManagersAsync()
        {
            return await _userRepository.GetUsersByTypeAsync("Manager");
        }

        public async Task<IEnumerable<User>> GetClientsAsync()
        {
            return await _userRepository.GetUsersByTypeAsync("Client");
        }
        public async Task<IEnumerable<User>> GetManagersWithClientsAsync()
        {
            return await _userRepository.GetManagersWithClientsAsync();
        }

        public async Task<IEnumerable<User>> GetClientsWithManagersAsync()
        {
            return await _userRepository.GetClientsWithManagersAsync();
        }

        public async Task<IEnumerable<User>> GetClientsForManagerAsync(string username)
        {
            return await _userRepository.GetClientsForManagerAsync(username);
        }

        public async Task<bool> AssignManagerAsync(int clientId, int managerId)
        {
            return await _userRepository.AssignManagerAsync(clientId, managerId);
        }
        

        public async Task<bool> ReassignClientManagerAsync(int clientId, int newManagerId)
        {
            return await _userRepository.ReassignClientManagerAsync(clientId, newManagerId);
        }

    }
}
