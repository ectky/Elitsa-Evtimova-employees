# Employee Pair Finder

A full-stack application that identifies the pair of employees who have worked together on common projects for the longest period of time.

Built as a task solution for **Sirma Group Holding**.

---

## Tech Stack

| Layer    | Technology          |
|----------|---------------------|
| Frontend | React + TypeScript (Vite) |
| Backend  | .NET Web API (C#)   |

---

## Project Structure

```
/
├── EmployeePairApi/              # .NET backend
│   ├── Controllers/
│   │   └── EmployeesController.cs
│   ├── Models/
│   │   ├── EmployeeRecord.cs
│   │   └── PairResult.cs
│   ├── Services/
│   │   ├── CsvParser.cs
│   │   └── EmployeePairCalculator.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   └── Program.cs
│
└── employee-pair-finder/         # React frontend
    └── src/
        ├── App.tsx
        └── App.css
```

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (LTS)
- [.NET SDK](https://dotnet.microsoft.com/download) (8.0+)

---

### Running the Backend

```bash
cd LongestPeriodAPI
dotnet run
```

API will start at `https://localhost:7040;http://localhost:5188`

---

### Running the Frontend

```bash
cd longest-period
npm install
npm run dev
```

App will start at `http://localhost:5173`

> Both terminals must be open at the same time for the app to work.

---

## How It Works

1. User uploads a CSV file through the UI
2. The file is sent to the .NET API via a `POST /api/employees/upload` request
3. The API parses the CSV, calculates overlapping work periods for every pair of employees across all projects, and returns the pair with the highest total days together
4. The React frontend displays the winning pair and a breakdown of their shared projects

---

## CSV Format

```
EmpID, ProjectID, DateFrom, DateTo
143, 12, 2013-11-01, 2014-01-05
218, 10, 2012-05-16, NULL
143, 10, 2009-01-01, 2011-04-27
```

- `DateTo` can be `NULL` — treated as today's date
- The following date formats are supported:

| Format | Example |
|---|---|
| `YYYY-MM-DD` | 2013-11-01 |
| `DD/MM/YYYY` | 01/11/2013 |
| `MM/DD/YYYY` | 11/01/2013 |
| `DD.MM.YYYY` | 01.11.2013 |
| `DD-MM-YYYY` | 01-11-2013 |
| `MMMM D, YYYY` | November 1, 2013 |
| `MMM DD YYYY` | Nov 01 2013 |
| `YYYY/MM/DD` | 2013/11/01 |

---

## API Endpoint

### `POST /api/employees/upload`

**Request:** `multipart/form-data` with a `file` field containing the CSV.

**Success response `200`:**
```json
{
  "emp1": 143,
  "emp2": 218,
  "totalDays": 8,
  "projects": [
    { "projectID": 10, "daysWorked": 8 }
  ]
}
```

**Error responses:**
| Status | Reason |
|---|---|
| `400` | No file uploaded or CSV could not be parsed |
| `404` | No overlapping periods found between any pair |

---

## Sample Files

Sample CSV files for testing are available in the `/samples` directory:

| File | Purpose |
|---|---|
| `basic.csv` | Standard ISO dates, simple case |
| `null_dates.csv` | Employees with NULL DateTo (still active) |
| `mixed_date_formats.csv` | Multiple date formats in one file |
| `no_overlap.csv` | No overlapping periods — tests error handling |
| `realistic.csv` | Larger dataset with mixed scenarios |
| `edge_duplicate_emp.csv` | Same employee listed twice on same project |
