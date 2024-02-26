namespace RHF.Shared;

public class DonationsHeaderDTO
{
    public int Id { get; set; }

    public string Recipient { get; set; } = null!;

    public string Period { get; set; } = null!;

    public double TotalAmount { get; set; }
    public virtual ICollection<DonationsDetail> DonationsDetails { get; set; } = new List<DonationsDetail>();
}