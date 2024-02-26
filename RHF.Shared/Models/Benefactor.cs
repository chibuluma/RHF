namespace RHF.Shared;

public partial class Benefactor
{
    public int Id { get; set; }

    public string Nrc { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;
    public string Gender { get; set; }=null!;
 
    public string PhoneNumber { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public virtual ICollection<BenefactorContribution> BenefactorContributions { get; set; } = new List<BenefactorContribution>();
}
