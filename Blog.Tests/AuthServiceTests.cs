using Blog.Application.Dto;
//using Blog.Application.Interface;
using Blog.Application.Services;
using Blog.Domain;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Blog.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<IUserProfileRepository> _profileRepo;
    private readonly Mock<JwtService> _jwtService;
    private readonly Mock<ILogger<AuthService>> _logger;
    private readonly Mock<AppDbContext> _context;
    private readonly IUnitOfWork _uow;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase(databaseName: "TestDb")
    .Options;

        _userRepo = new Mock<IUserRepository>();
        _profileRepo = new Mock<IUserProfileRepository>();
        _jwtService = new Mock<JwtService>();
        _logger = new Mock<ILogger<AuthService>>();
        _context = new Mock<AppDbContext>(options);
        _uow = new UnitOfWork(_context.Object);

        _authService = new AuthService(
            _userRepo.Object,
            _profileRepo.Object,
            _jwtService.Object,
            _logger.Object,
            _uow
        );
    }


    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndProfile()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "john",
            Email = "john@test.com",
            Password = "123456"
        };

        _userRepo.Setup(r => r.GetByUsernameAsync(dto.Username))
                 .ReturnsAsync((User)null);

        _userRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                 .ReturnsAsync((User)null);

        var result = await _authService.RegisterAsync(dto);

        object value = result.Should().Be("Success");

        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _profileRepo.Verify(r => r.AddAsync(It.IsAny<UserProfile>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenUsernameExists()
    {
        var dto = new RegisterDto
        {
            Username = "john",
            Email = "john@test.com",
            Password = "123456"
        };

        _userRepo.Setup(r => r.GetByUsernameAsync(dto.Username))
                 .ReturnsAsync(new User());

        var result = await _authService.RegisterAsync(dto);

        result.Should().Be("Username already exists");
    }


    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenPasswordCorrect()
    {
        var password = "123456";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john",
            PasswordHash = hash
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("john"))
                 .ReturnsAsync(user);

        _jwtService.Setup(j => j.GenerateJwtToken(user))
                   .Returns("fake-jwt");

        var token = await _authService.LoginAsync("john", password);

        token.Should().Be("fake-jwt");
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenPasswordWrong()
    {
        var user = new User
        {
            Username = "john",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct")
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("john"))
                 .ReturnsAsync(user);

        var token = await _authService.LoginAsync("john", "wrong");

        token.Should().Be("Invalid credentials");
    }
}
