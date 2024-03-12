public class Metrics{
    public double TotalBenefactors { get; set; }
    public double TotalContributions { get; set; }
    public double MyTotalContributions { get; set; }
    public double[] ContributedVsRaised { get; set; } = new double [2];
    public DateTime NextProjectSchedule { get; set; }

}