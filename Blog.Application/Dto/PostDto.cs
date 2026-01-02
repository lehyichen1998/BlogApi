using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Dto;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public List<string> Tags { get; set; }
}
