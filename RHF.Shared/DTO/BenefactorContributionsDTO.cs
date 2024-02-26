using System.ComponentModel.DataAnnotations;

namespace RHF.Shared;
public class BenefactorContributionsDTO {
    public int Id { get; set; }

    public int BenefactorId { get; set; }
    [Required]
    public DateTime? DatePaid { get; set; } = DateTime.Now;
    [Required]
    public double AmountPaid { get; set; }
}