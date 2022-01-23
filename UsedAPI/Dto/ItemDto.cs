using System.ComponentModel.DataAnnotations;

namespace UsedAPI.Dto;

public class ItemDto
{
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;
    
    public double Price { get; set; }

    public string Category { get; set; } = string.Empty;

    [StringLength(200)] 
    public string Description { get; set; } = string.Empty;

    public string PicUrl { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
    
    public string Condition { get; set; } = string.Empty;
}