namespace RHF.Shared;

public class DonationsDetailDTO
{
    public int Id { get; set; }

    public int DonationsHeaderId { get; set; }

    public string Description { get; set; } = null!;

    public int Qty { get; set; }

    public double? UnitCost { get; set; }

    public double? Total { get; set; }
}