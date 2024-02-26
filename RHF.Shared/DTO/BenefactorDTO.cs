using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;
using RHF.Shared;

public class BenefactorDTO{
    public int Id { get; set; }
    [Required]
    [RegularExpression(@"\d{6}/\d{1,2}/\d{1,2}$", ErrorMessage ="NRC number not in the correct format")]
    public string Nrc { get; set; } = null!;
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    [Length(3, 15)]
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    [Required]
    [Length(3, 15)]
    public string LastName { get; set; } = null!;
    [Required]
    public string Gender { get; set; } = null!;
    [Required]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Phone number not in the correct format")]
    public string PhoneNumber { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public virtual ICollection<BenefactorContribution> BenefactorContributions { get; set; } = new List<BenefactorContribution>();

}