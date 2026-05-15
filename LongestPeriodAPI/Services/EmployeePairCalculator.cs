using LongestPeriodAPI.Models;

namespace LongestPeriodAPI.Services
{
    public static class EmployeePairCalculator
    {
        public static PairResult? FindLongestPair(List<EmployeeRecord> records)
        {
            // Group by project
            var byProject = records.GroupBy(r => r.ProjectID);

            // pairKey -> (totalDays, list of project overlaps)
            var pairMap = new Dictionary<string, PairResult>();

            foreach (var project in byProject)
            {
                var members = project.ToList();

                for (int i = 0; i < members.Count; i++)
                {
                    for (int j = i + 1; j < members.Count; j++)
                    {
                        var a = members[i];
                        var b = members[j];

                        var overlapStart = a.DateFrom > b.DateFrom ? a.DateFrom : b.DateFrom;
                        var overlapEnd = a.DateTo < b.DateTo ? a.DateTo : b.DateTo;

                        if (overlapEnd <= overlapStart) continue;

                        var days = (int)(overlapEnd - overlapStart).TotalDays;

                        // Always sort so (1,2) and (2,1) are the same key
                        var e1 = Math.Min(a.EmpID, b.EmpID);
                        var e2 = Math.Max(a.EmpID, b.EmpID);
                        var key = $"{e1}|{e2}";

                        if (!pairMap.ContainsKey(key))
                            pairMap[key] = new PairResult(e1, e2) { Emp1 = e1, Emp2 = e2 };

                        pairMap[key].TotalDays += days;
                        pairMap[key].Projects.Add(new ProjectOverlap(project.Key, days));
                    }
                }
            }

            if (!pairMap.Any()) return null;

            return pairMap.Values.OrderByDescending(p => p.TotalDays).First();
        }
    }
}
