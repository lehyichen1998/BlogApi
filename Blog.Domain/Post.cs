using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain;

public class Post
{
    public Guid Id { get; set;  }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public Boolean IsActive { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ICollection<PostTag> PostTags { get; set; }
}
