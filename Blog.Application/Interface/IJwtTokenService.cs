using Blog.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Interface;

public interface IJwtTokenService
{
    string GenerateJwtToken(User user);
}