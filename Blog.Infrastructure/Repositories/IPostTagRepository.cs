using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public interface IPostTagRepository
{
    Task AddAsync(PostTag postTag);
    void Delete(PostTag postTag);
    Task RemoveByPostIdAsync(Guid postId);

}
