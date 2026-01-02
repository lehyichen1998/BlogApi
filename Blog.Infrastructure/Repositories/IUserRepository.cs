using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);

    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<List<User>> GetAllWithProfilesAsync();
    void Update(User user);

    void Delete(User user);
}
