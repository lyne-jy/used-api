#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UsedAPI.Models;

public class Item
{
    public int Id { get; set; }
    
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;
    
    public double Price { get; set; }
    
    public string Category { get; set; } = string.Empty;

    [StringLength(200)] 
    public string Description { get; set; } = string.Empty;

    public string PicUrl { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime? Date { get; set; }

    public string Condition { get; set; } = string.Empty;
    
    public int SellerId { get; set; }
}