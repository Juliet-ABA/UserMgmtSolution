using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserMgmt.Core.Models;
using UserMgmt.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace UserMgmt.API.Controllers
{
    [Route("api/[controller]")] // Configure base route for user management
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/users/search
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Please provide a search term");
            }

            try
            {
                var users = await _userService.SearchUsersAsync(searchTerm);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/users/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        // GET: api/users/managers
        [HttpGet]
        [Route("managers")]
        public async Task<IActionResult> GetManagers()
        {
            try
            {
                var managers = await _userService.GetManagersAsync();
                return Ok(managers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/users/managers-with-clients
        [HttpGet]
        [Route("managers-with-clients")]
        public async Task<IActionResult> GetManagersWithClients()
        {
            try
            {
                var managersWithClients = await _userService.GetManagersWithClientsAsync();
                return Ok(managersWithClients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/users/clients
        [HttpGet]
        [Route("clients")]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                var clients = await _userService.GetClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        // GET: api/users/clients-with-managers
        [HttpGet]
        [Route("clients-with-managers")]
        public async Task<IActionResult> GetClientsWithManagers()
        {
            try
            {
                var clientsWithManagers = await _userService.GetClientsWithManagersAsync();
                return Ok(clientsWithManagers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        // GET: api/users/manager/{username}/clients
        [HttpGet]
        [Route("manager/{username}/clients")]
        public async Task<IActionResult> GetClientsForManager(string username)
        {
            try
            {
                var clients = await _userService.GetClientsForManagerAsync(username);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                User userToAdd= null;

                if (createUserDto.UserType == "Manager")
                {
                    // Prompt for Manager-specific properties (position)
                    if (string.IsNullOrEmpty(createUserDto.Position))
                    {
                        return BadRequest("Position is required for Manager type");
                    }

                    var manager = new Manager
                    {
                        UserName = createUserDto.UserName,
                        Email = createUserDto.Email,
                        Alias = createUserDto.Alias,
                        FirstName = createUserDto.FirstName,
                        LastName = createUserDto.LastName,
                        UserType=createUserDto.UserType,

                        // Set other properties for Manager
                    };
                    manager.Position = createUserDto.Position;
                    userToAdd = manager;
                }
                else if (createUserDto.UserType == "Client")
                {
                    // Prompt for Client-specific properties (level)
                    if (createUserDto.Level<=0)
                    {
                        return BadRequest("Level is required for Client type");
                    }

                    var client = new Client
                    {
                        UserName = createUserDto.UserName,
                        Email = createUserDto.Email,
                        Alias = createUserDto.Alias,
                        FirstName = createUserDto.FirstName,
                        LastName = createUserDto.LastName,
                        UserType=createUserDto.UserType,

                    };
                    client.Level = createUserDto.Level;
                    userToAdd = client;
                }
                else
                {
                    return BadRequest("Invalid user type provided");
                }

                if (userToAdd == null)
                {
                    return BadRequest("User object could not be created");
                }

                await _userService.AddUserAsync(userToAdd);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message); // 400 Bad Request for specific errors
            }
        }

        // PUT: api/users/{id}
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                User userToAdd = null;


                    userToAdd = new User
                    {
                        UserName = updateUserDto.UserName,
                        Email = updateUserDto.Email,
                        Alias = updateUserDto.Alias,
                        FirstName = updateUserDto.FirstName,
                        LastName = updateUserDto.LastName,

                        // Set other properties for Manager
                    };


                userToAdd.UserId = id;
                await _userService.UpdateUserAsync(userToAdd);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message); // 400 Bad Request for specific errors
            }
        }


        // DELETE: api/users/{id}
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (await _userService.DeleteUserAsync(id))
                {
                    return NoContent(); // 204 No Content (successful deletion)
                }

                return NotFound(); // Indicate user not found for deletion
            }
            catch (Exception ex)
            {
                // Handle potential exceptions during user deletion
                if (ex is DbUpdateException) // Example: Database update exception
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "An error occurred while deleting the user. Please ensure no related entities exist.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        [HttpPost("assign-manager")]
        public async Task<ActionResult> AssignManager(AssignManagerRequest request)
        {
            try
            {
                var result = await _userService.AssignManagerAsync(request.ClientId, request.ManagerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("reassign-client-manager")]
        public async Task<ActionResult> ReassignClientManager(ReassignClientManagerRequest request)
        {
            try
            {
                var result = await _userService.ReassignClientManagerAsync(request.ClientId, request.NewManagerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Request models for  controller actions
        public class AssignManagerRequest
        {
            public int ClientId { get; set; }
            public int ManagerId { get; set; }
        }

        public class ReassignClientManagerRequest
        {
            public int ClientId { get; set; }
            public int NewManagerId { get; set; }
        }
    }
}
