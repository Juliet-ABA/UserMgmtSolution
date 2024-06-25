using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserMgmt.API.Controllers;
using UserMgmt.Core.Models;
using UserMgmt.Core;
using Xunit;
using Moq;
using System.Linq;

namespace UserMgmtAPITests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IUserService> _mockUserService = new Mock<IUserService>();

        public UserControllerTests()
        {
            _controller = new UserController(_mockUserService.Object);
        }

        // Test for GET: api/users
        [Fact]
        public async Task GetUsers_ReturnsListOfUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
    {
        new User { UserId = 1, UserName = "User1", Email = "user1@example.com" },
        new User { UserId = 2, UserName = "User2", Email = "user2@example.com" }

    };

            _mockUserService.Setup(x => x.GetAllUsersAsync())
                            .ReturnsAsync(expectedUsers); 

            // Act
            var result = await _controller.GetUsers() as IActionResult;

            // Assert
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);

            Assert.Equal(expectedUsers.Count, actualUsers.Count());

        }


        // Test for GET: api/users/{id}
        [Fact]
        public async Task GetUserById_ExistingId_ReturnsUser()
        {
            // Arrange
            int userId = 1;
            var user = new User { UserId = userId, UserName = "User1", UserType = "Manager" };
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userId, returnedUser.UserId);
        }

        // Test for POST: api/users
        [Fact]
        public async Task AddUser_ValidUser_ReturnsCreatedAtRoute()
        {
            // Arrange
            var createUserDto = new AddUserDto
            {
                UserName = "NewUser",
                Email = "newuser@example.com",
                Alias = "NU",
                FirstName = "New",
                LastName = "User",
                UserType = "Manager",
                Position = "Senior Manager" 
            };

            _mockUserService.Setup(service => service.AddUserAsync(It.IsAny<User>())).ReturnsAsync(new User { UserId = 1 });

            // Act
            var result = await _controller.AddUser(createUserDto);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal("GetUserById", createdAtRouteResult.RouteName);
            Assert.Equal(1, createdAtRouteResult.RouteValues["id"]);
        }

        // Test for PUT: api/users/{id}
        [Fact]
        public async Task UpdateUser_ExistingId_ValidUser_ReturnsNoContent()
        {
            // Arrange
            int userId = 1;
            var updateUserDto = new UpdateUserDto
            {
                UserName = "UpdatedUserName",
                Email = "updated@example.com",
                Alias = "UpdatedAlias",
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"

            };

            var updatedUser = new User
            {
                UserId = userId,
                UserName = updateUserDto.UserName,
                Email = updateUserDto.Email,
                Alias = updateUserDto.Alias,
                FirstName = updateUserDto.FirstName,
                LastName = updateUserDto.LastName

            };

            _mockUserService.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                            .ReturnsAsync(updatedUser); 

            // Act
            var result = await _controller.UpdateUser(userId, updateUserDto) as IActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        // Test for DELETE: api/users/{id}
        [Fact]
        public async Task DeleteUser_ExistingId_ReturnsNoContent()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(service => service.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        
        // Test for POST: api/users/assign-manager
        [Fact]
        public async Task AssignManager_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new UserController.AssignManagerRequest
            {
                ClientId = 1,
                ManagerId = 2
            };

            _mockUserService.Setup(service => service.AssignManagerAsync(request.ClientId, request.ManagerId)).ReturnsAsync(true);

            // Act
            var result = await _controller.AssignManager(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task GetManagersWithClientsAsync_ReturnsManagersWithClients()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var managersWithClients = new List<User>
            {
                new User { UserId = 1, UserName = "Manager1", UserType = "Manager", ClientRelationships = new List<UserRelationship>
                    {
                        new UserRelationship { ClientId = 101, Client = new User { UserId = 101, UserName = "Client1", UserType = "Client" } }
                    }
                }
            };
            mockUserService.Setup(service => service.GetManagersWithClientsAsync()).ReturnsAsync(managersWithClients);
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.GetManagersWithClients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Single(model); // Ensure one manager is returned
            var manager = model.First();
            Assert.Equal("Manager1", manager.UserName);
            Assert.Single(manager.ClientRelationships); // Ensure the manager has one client
            var client = manager.ClientRelationships.First().Client;
            Assert.Equal("Client1", client.UserName);
        }
        [Fact]
        public async Task GetClientsWithManagersAsync_ReturnsListOfClientsWithManagers()
        {
            // Arrange
            var expectedClients = new List<User>
    {
        new User { UserId = 3, UserName = "Client1", UserType = "Client" },
        new User { UserId = 4, UserName = "Client2", UserType = "Client" }
    };

            // Mocking service call
            _mockUserService.Setup(service => service.GetClientsWithManagersAsync())
                .ReturnsAsync(expectedClients);

            // Act
            var result = await _controller.GetClientsWithManagers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(expectedClients.Count, model.Count());
            Assert.Equal(expectedClients[0].UserId, model.ToList()[0].UserId); 
            Assert.Equal(expectedClients[1].UserName, model.ToList()[1].UserName); 
        }



        // Test for PUT: api/users/reassign-client-manager
        [Fact]
        public async Task ReassignClientManager_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new UserController.ReassignClientManagerRequest
            {
                ClientId = 1,
                NewManagerId = 2
            };

            _mockUserService.Setup(service => service.ReassignClientManagerAsync(request.ClientId, request.NewManagerId)).ReturnsAsync(true);

            // Act
            var result = await _controller.ReassignClientManager(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }
    }
}
