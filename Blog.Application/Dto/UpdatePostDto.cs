using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Dto;

public class UpdatePostDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public List<Guid> TagIds { get; set; } = new();
}
