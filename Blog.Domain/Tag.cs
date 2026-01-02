using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public Boolean IsActive { get; set; }
    public ICollection<PostTag> PostTags { get; set; }
}
