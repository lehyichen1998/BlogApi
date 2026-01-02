using Blog.Application.Dto;
using Blog.Domain;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;
namespace Blog.Application.Services;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IPostTagRepository _postTagRepository;
    private readonly IUnitOfWork _uow;

    public PostService(IPostRepository postRepository, 
        ITagRepository tagRepository,
        IPostTagRepository postTagRepository,
        IUnitOfWork uow)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _postTagRepository = postTagRepository;
        _uow = uow;
    }

    public async Task<PostResponseDto> CreatePostAsync(Guid userId, string title, string content, List<Guid> tagIds)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

            await _postRepository.AddAsync(post);
            await _uow.SaveChangesAsync();

            foreach (var tagId in tagIds.Distinct())
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag == null)
                    throw new Exception($"Invalid tagId: {tagId}");

                await _postTagRepository.AddAsync(new PostTag
                {
                    PostId = post.Id,
                    TagId = tag.Id
                });
            }

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Tags = post.PostTags.Select(pt => new TagDto
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name
                }).ToList()
            };
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Post>> GetPostsByUserAsync(Guid userId)
    {
        return await _postRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _postRepository.GetAllAsync();
    }

    public async Task<Post> GetPostByIdAsync(Guid postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new Exception("Post not found");

        return post;
    }

    public async Task<string> UpdatePostAsync(Guid postId, string title, string content, List<Guid> tagIds)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new Exception("Post not found");

            post.Title = title;
            post.Content = content;

            _postRepository.Update(post);

            await _postTagRepository.RemoveByPostIdAsync(postId);

            foreach (var tagId in tagIds.Distinct())
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag == null)
                    throw new Exception($"Invalid tagId: {tagId}");

                await _postTagRepository.AddAsync(new PostTag
                {
                    PostId = post.Id,
                    TagId = tag.Id
                });
            }

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return "Post updated successfully";
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
    public async Task<string> DeletePostAsync(Guid postId)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new Exception("Post not found");

            await _postTagRepository.RemoveByPostIdAsync(postId);
            _postRepository.Delete(post);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return "Post deleted successfully";
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}
