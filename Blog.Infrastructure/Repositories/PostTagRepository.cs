using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public class PostTagRepository : IPostTagRepository
{
    private readonly AppDbContext _context;
    public PostTagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PostTag postTag)
    {
        await _context.PostTags.AddAsync(postTag);
    }

    public void Delete(PostTag postTag)
    {
        _context.PostTags.Remove(postTag);
    }
    public async Task RemoveByPostIdAsync(Guid postId)
    {
        var links = _context.PostTags.Where(x => x.PostId == postId);
        _context.PostTags.RemoveRange(links);
        await _context.SaveChangesAsync();
    }
}
