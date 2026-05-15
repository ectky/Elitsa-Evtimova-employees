using LongestPeriodAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Globalization;

namespace LongestPeriodAPI.Services
{
    public static class CsvParser
    {
        // All date formats we support
        private static readonly string[] DateFormats =
        {
            "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy",
            "dd.MM.yyyy", "dd-MM-yyyy", "MMMM d, yyyy",
            "MMM dd yyyy", "yyyy/MM/dd"
        };

        public static List<EmployeeRecord> Parse([FromBody] IFormFile file)
        {
            var records = new List<EmployeeRecord>();
            using var reader = new StreamReader(file.OpenReadStream());

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split(',');
                if (cols.Length < 4) continue;

                if (!int.TryParse(cols[0].Trim(), out var empId)) continue;
                if (!int.TryParse(cols[1].Trim(), out var projectId)) continue;

                if (!TryParseDate(cols[2].Trim(), out var dateFrom)) continue;

                // NULL means today
                var dateToRaw = cols[3].Trim();
                DateTime dateTo = dateToRaw.Equals("NULL", StringComparison.OrdinalIgnoreCase)
                    ? DateTime.Today
                    : TryParseDate(dateToRaw, out var parsedTo) ? parsedTo : DateTime.Today;

                records.Add(new EmployeeRecord(empId, projectId, dateFrom, dateTo));
            }

            return records;
        }

        private static bool TryParseDate(string raw, out DateTime result)
        {
            return DateTime.TryParseExact(
                raw, DateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result);
        }
    }
}
