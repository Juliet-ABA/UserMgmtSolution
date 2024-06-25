using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserMgmt.Core.Models;

namespace UserMgmt.Core
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
        Task<IEnumerable<User>> GetManagersWithClientsAsync();
        Task<IEnumerable<User>> GetClientsWithManagersAsync();
        Task<IEnumerable<User>> GetClientsForManagerAsync(string username);

        Task<IEnumerable<User>> GetUsersByTypeAsync(string userType);
        Task<bool> AssignManagerAsync(int clientId, int managerId);
        Task<bool> ReassignClientManagerAsync(int clientId, int newManagerId);
    }

}
