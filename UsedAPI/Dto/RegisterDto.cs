using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UsedAPI.Dto;

public class RegisterDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}