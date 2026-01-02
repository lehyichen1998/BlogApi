using Blog.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;
    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
    }

    public async Task<Tag> GetByIdAsync(Guid id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
        return await _context.Tags.FirstOrDefaultAsync<Tag>(t => t.Name == name);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public void Update(Tag tag)
    {
        _context.Tags.Update(tag);
    }

    public void Delete(Tag tag)
    {
        _context.Tags.Remove(tag);
    }
}
