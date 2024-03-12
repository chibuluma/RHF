namespace RHF.Shared;
using System.ComponentModel;
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