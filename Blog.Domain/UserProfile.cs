using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain;

public class UserProfile
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string? ProfileUrl { get; set; }
    public string PhoneNo { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
