using System.ComponentModel.DataAnnotations;

namespace UsedAPI.Models;

public class Seller
{
    public int Id { get; set; }
    
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;
    
    public byte[] PasswordHash { get; set; }
    
    public byte[] PasswordSalt { get; set; }
    
    [EmailAddress]
    [StringLength(30)]
    public string Email { get; set; } = string.Empty;
    
    public ICollection<Item>? Items { get; set; }
}