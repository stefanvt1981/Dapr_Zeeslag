using System.ComponentModel.DataAnnotations;

namespace ZeeslagFrontEnd.Shared.Model;

public class PlayerModel
{
    [Required]
    [StringLength(10, ErrorMessage = "Name is too long.")]
    public string Name { get; set; }
        
    public int Age { get; set; }
}
