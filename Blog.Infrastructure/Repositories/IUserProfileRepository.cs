using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserProfile profile);
    void Update(UserProfile profile);
    void Delete(UserProfile profile);
}
