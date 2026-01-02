using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public Boolean IsActive { get; set; }
    public UserProfile Profile { get; set; }
    public ICollection<Post> Posts { get; set; }
}
