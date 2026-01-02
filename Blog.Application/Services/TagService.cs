using Blog.Domain;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
namespace Blog.Application.Services;

public class TagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<TagService> _logger;
    private readonly IUnitOfWork _uow;

    public TagService(ITagRepository tagRepository, ILogger<TagService> logger, IUnitOfWork uow)
    {
        _tagRepository = tagRepository;
        _logger = logger;
        _uow = uow;
    }

    public async Task<Tag> CreateTagAsync(string name)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            var existing = await _tagRepository.GetByNameAsync(name);
            if (existing != null)
                throw new Exception($"Tag '{name}' already exists.");

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system",
                IsActive = true
            };

            await _tagRepository.AddAsync(tag);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return tag;
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();

            _logger.LogError(ex, "Failed to create tag: {TagName}", name);
            throw;
        }
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        try
        {
            return await _tagRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all tags");
            throw;
        }
    }

    public async Task<Tag> UpdateTagAsync(Guid tagId, string newName)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null)
                throw new Exception("Tag not found.");

            tag.Name = newName;
            _tagRepository.Update(tag);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return tag;
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();

            _logger.LogError(ex, "Failed to update tag: {TagId}", tagId);
            throw;
        }
    }

    public async Task DeleteTagAsync(Guid tagId)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null)
                throw new Exception("Tag not found.");

            _tagRepository.Delete(tag);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();

            _logger.LogError(ex, "Failed to delete tag: {TagId}", tagId);
            throw;
        }
    }
}
