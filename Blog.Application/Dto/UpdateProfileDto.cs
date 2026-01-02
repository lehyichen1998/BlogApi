using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Dto;

public class UpdateProfileDto
{
    public string Description { get; set; }
    public string ProfileUrl { get; set; }
    public string PhoneNo { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public bool IsActive { get; set; }
}
