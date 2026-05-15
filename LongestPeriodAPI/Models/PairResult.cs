namespace LongestPeriodAPI.Models
{
    public record ProjectOverlap(int ProjectID, int DaysWorked);

    public record PairResult(int Emp1, int Emp2)
    {
        public int TotalDays { get; set; } = 0;
        public List<ProjectOverlap> Projects { get; set; } = new();
    }
}
