namespace  RHF.Shared;

using System.ComponentModel.DataAnnotations;
using RHF.Shared;

public class FinancialYear {
    [Key]
    public int Id { get; set; }
    public int Year { get; set; }
    public bool IsCurrent { get; set; }

}