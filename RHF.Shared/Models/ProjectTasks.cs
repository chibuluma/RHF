namespace RHF.Shared;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public enum FillStyleEnum
{
    [Description("fill")]
    Fill,
    [Description("backwardDiagonal")]
    BackwardDiagonal,
    [Description("zigZag")]
    ZigZag,
    [Description("triangles")]
    Triangles,
    [Description("crossDots")]
    CrossDots
}
public sealed class ProjectTasks
{
    [Key]
    public int ID { get; set; }

    public string? Key { get; set; }
    [Required]
    public string Caption { get; set; }
    [Required]
    public string Code { get; set; }
    [Required]
    public string Color { get; set; }

    public string? ForeColor { get; set; }

    public FillStyleEnum FillStyle { get; set; }
    [Required]
    public string? Comment { get; set; }
    [Required]
    public DateTime DateStart { get; set; }
    [Required]
    public DateTime DateEnd { get; set; }

    public bool NotBeDraggable { get; set; }

    public int Type { get; set; }
    public string CreatedBy { get; set; }
}