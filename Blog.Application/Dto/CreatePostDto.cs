using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Dto;

public class CreatePostDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    //public List<string> Tags { get; set; }
    public List<Guid> TagIds { get; set; } = new();
}
