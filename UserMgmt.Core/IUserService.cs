using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserMgmt.Core.Models;

namespace UserMgmt.Core
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<User>> GetManagersAsync();
        Task<IEnumerable<User>> GetClientsAsync();
        Task<IEnumerable<User>> GetManagersWithClientsAsync();
        Task<IEnumerable<User>> GetClientsWithManagersAsync();
        Task<IEnumerable<User>> GetClientsForManagerAsync(string username);
        Task<bool> AssignManagerAsync(int clientId, int managerId);
        Task<bool> ReassignClientManagerAsync(int clientId, int newManagerId);

    }

}
