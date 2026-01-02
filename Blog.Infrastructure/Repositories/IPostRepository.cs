using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public interface IPostRepository
{
    Task AddAsync(Post post);

    Task<Post> GetByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetByUserIdAsync(Guid userId);

    void Update(Post post);

    void Delete(Post post);
}
