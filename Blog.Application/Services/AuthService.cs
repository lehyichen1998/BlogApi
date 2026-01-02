using Blog.Application.Dto;
using Blog.Domain;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
namespace Blog.Application.Services;


public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _profileRepository;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;
    private readonly IUnitOfWork _uow;

    public AuthService(
            IUserRepository userRepository,
            IUserProfileRepository profileRepository,
            JwtService jwtService,
            ILogger<AuthService> logger,
            IUnitOfWork uow)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _jwtService = jwtService;
        _logger = logger;
        _uow = uow;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            if (await _userRepository.GetByUsernameAsync(dto.Username) != null)
                throw new Exception("Username already exists");

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
                throw new Exception("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Description = dto.Description ?? "",
                ProfileUrl = dto.ProfileUrl ?? "",
                PhoneNo = dto.PhoneNo ?? ""
            };
            await _profileRepository.AddAsync(profile);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return "Success";
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();

            _logger.LogError(ex,
                "Registration failed for Email={Email}, Username={Username}",
                dto.Email, dto.Username);

            throw;
        }
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            return _jwtService.GenerateJwtToken(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while logging in for username: {Username}", username);

            return ex.Message;
        }

    }
}
