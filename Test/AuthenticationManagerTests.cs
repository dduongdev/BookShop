using Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases;
using UseCases.Repositories;
using UseCases.TaskResults;

namespace Test
{
    public class AuthenticationManagerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthenticationManager _authenticationManager;

        public AuthenticationManagerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authenticationManager = new AuthenticationManager(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsUserNotFound()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";
            _userRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _authenticationManager.LoginAsync(username, password);

            // Assert
            Assert.Equal(LoginResultCodes.UserNotFound, result.ResultCode);
            Assert.Null(result.User);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ShoundReturnWrongPassword()
        {
            var storedUser = new User
            {
                Email = "",
                Phone = "",
                Username = "dduongdev",
                Password = "040313"
            };
            var passwordHasher = new PasswordHasher<string>();
            storedUser.Password = passwordHasher.HashPassword(storedUser.Username, storedUser.Password);
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<User> 
            {
                storedUser
            });

            var username = "dduongdev";
            var password = "bruh";

            var result = await _authenticationManager.LoginAsync(username, password);

            Assert.Equal(LoginResultCodes.WrongPassword, result.ResultCode);
            Assert.Null(result.User);
            Assert.Equal("Wrong password!", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess()
        {
            var storedUser = new User
            {
                Email = "",
                Phone = "",
                Username = "dduongdev",
                Password = "040313"
            };
            var passwordHasher = new PasswordHasher<string>();
            storedUser.Password = passwordHasher.HashPassword(storedUser.Username, storedUser.Password);
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<User>
            {
                storedUser
            });

            var username = "dduongdev";
            var password = "040313";

            var result = await _authenticationManager.LoginAsync(username, password);

            Assert.Equal(LoginResultCodes.Success, result.ResultCode);
            Assert.NotNull(result.User);
            Assert.Equal("Success!", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError()
        {
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception());

            var username = "testusername";
            var password = "testpassword";

            var result = await _authenticationManager.LoginAsync(username, password);

            Assert.Equal(LoginResultCodes.Error, result.ResultCode);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task SignupAsync_UserExisted_ReturnsUserExisted()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "testpassword", Email = "", Phone = "" };
            _userRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<User> { user });

            // Act
            var result = await _authenticationManager.SignupAsync(user);

            // Assert
            Assert.Equal(SignupResult.UserExisted, result);
        }

        [Fact]
        public async Task SignupAsync_Success_ReturnsSuccess()
        {
            // Arrange
            var user = new User { Username = "newuser", Password = "newpassword", Email = "", Phone = "" };
            _userRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _authenticationManager.SignupAsync(user);

            // Assert
            Assert.Equal(SignupResult.Success, result);
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task SignupAsync_Error_ReturnsError()
        {
            // Arrange
            var user = new User { Username = "newuser", Password = "newpassword", Email = "", Phone = "" };
            _userRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _authenticationManager.SignupAsync(user);

            // Assert
            Assert.Equal(SignupResultCodes.Error, result.ResultCode);
            Assert.Equal("Database error", result.Message);
        }
    }
}
