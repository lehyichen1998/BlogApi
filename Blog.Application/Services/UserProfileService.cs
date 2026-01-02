using Blog.Application.Dto;
using Blog.Domain;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
namespace Blog.Application.Services;

public class UserProfileService
{
    private readonly IUserProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public UserProfileService(IUserProfileRepository profileRepository, IUserRepository userRepository, AppDbContext context, ILogger<AuthService> logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfile> GetProfileAsync(Guid userId)
    {
        return await _profileRepository.GetByUserIdAsync(userId);
    }

    public async Task<string> UpdateProfileAsync(UpdateProfileDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (await _userRepository.GetByUsernameAsync(dto.Username) != null)
                throw new Exception("Username already exists");

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
                throw new Exception("Email already exists");

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("User not found");
            user.Profile.Description = dto.Description;
            user.Profile.ProfileUrl = dto.ProfileUrl;
            user.Profile.PhoneNo = dto.PhoneNo;
            user.Email = dto.Email;
            user.Username = dto.Username;
            user.IsActive = dto.IsActive;
            _userRepository.Update(user);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return "Profile updated successfully";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Update failed for Email={Email}, Username={Username}",
                dto.Email, dto.Username);

            return ex.Message;
        }
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllWithProfilesAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Description = u.Profile?.Description,
            ProfileUrl = u.Profile?.ProfileUrl,
            PhoneNo = u.Profile?.PhoneNo
        }).ToList();
    }

    public async Task<string> DeleteProfileAsync(Guid userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var profile = await _userRepository.GetByIdAsync(userId);
            if (profile == null)
            {
                throw new Exception("Please make sure the UserId is correct!");
            }
            _userRepository.Delete(profile);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return "Profile deleted successfully";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return $"Error deleting profile: {ex.Message}";
        }
    }
}
