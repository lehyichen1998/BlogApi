using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public interface ITagRepository
{
    Task AddAsync(Tag tag);
    Task<Tag> GetByIdAsync(Guid id);
    Task<Tag> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetAllAsync();
    void Update(Tag tag);
    void Delete(Tag tag);
}
