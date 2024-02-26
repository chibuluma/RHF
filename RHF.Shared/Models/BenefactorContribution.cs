namespace RHF.Shared;

public partial class BenefactorContribution
{
    public int Id { get; set; }

    public int BenefactorId { get; set; }

    public DateTime DatePaid { get; set; }

    public double AmountPaid { get; set; }

    public virtual Benefactor Benefactor { get; set; } = null!;
}
