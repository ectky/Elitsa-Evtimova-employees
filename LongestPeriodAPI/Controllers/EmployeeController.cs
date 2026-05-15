using LongestPeriodAPI.Models;
using LongestPeriodAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LongestPeriodAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var records = CsvParser.Parse(file);
            if (!records.Any())
                return BadRequest("Could not parse any valid rows from the CSV.");

            var result = EmployeePairCalculator.FindLongestPair(records);
            if (result == null)
                return NotFound("No overlapping periods found between any pair.");

            return Ok(result);
        }
    }
}