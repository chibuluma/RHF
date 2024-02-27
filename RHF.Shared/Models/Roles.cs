using System.ComponentModel.DataAnnotations;

public class Roles {
    public string Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string ConcurrencyStamp { get; set; }

}