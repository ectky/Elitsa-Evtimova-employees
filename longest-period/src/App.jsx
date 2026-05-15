import { useState, useCallback } from "react";
import './App.css';

// ── component ────────────────────────────────────────────────────────────────
export default function App() {
  const [dragging, setDragging]   = useState(false);
  const [fileName, setFileName]   = useState("");
  const [result,   setResult]     = useState(null);
  const [error,    setError]      = useState("");
  const [loading,  setLoading]    = useState(false);

  const processFile = useCallback((file) => {
    if (!file || !file.name.endsWith(".csv")) {
      setError("Please upload a valid .csv file.");
      return;
    }
    setError("");
    setResult(null);
    setLoading(true);
    setFileName(file.name);

    // Send the file to the .NET API
    const formData = new FormData();
    formData.append("file", file);

    fetch("https://localhost:7040/api/employees/upload", {
      method: "POST",
      body: formData,
    })
      .then((res) => {
        if (!res.ok) return res.text().then((t) => { throw new Error(t); });
        return res.json();
      })
      .then((data) => {
        setLoading(false);
        // API returns camelCase by default in .NET
        setResult({
          emp1: data.emp1,
          emp2: data.emp2,
          totalDays: data.totalDays,
          projects: data.projects.map((p) => ({
            projectId: String(p.projectID),
            days: p.daysWorked,
          })),
        });
      })
      .catch((err) => {
        setLoading(false);
        setError(err.message || "Something went wrong.");
      });
  }, []);

  const onFileChange = (e) => processFile(e.target.files[0]);
  const onDrop = (e) => {
    e.preventDefault();
    setDragging(false);
    processFile(e.dataTransfer.files[0]);
  };

  return (
    <>
      <div className="app">
        {/* Header */}
        <div className="header">
          <div className="logo">S</div>
          <div className="header-text">
            <h1>Longest period</h1>
            <p>SIRMA GROUP HOLDING — TASK SOLUTION</p>
          </div>
        </div>

        {/* Upload */}
        <div
          className={`upload-zone${dragging ? " drag" : ""}`}
          onDragOver={(e) => { e.preventDefault(); setDragging(true); }}
          onDragLeave={() => setDragging(false)}
          onDrop={onDrop}
        >
          <input type="file" accept=".csv" onChange={onFileChange} />
          <div className="upload-icon">
            <svg viewBox="0 0 24 24" fill="none" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
              <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
              <polyline points="17 8 12 3 7 8"/>
              <line x1="12" y1="3" x2="12" y2="15"/>
            </svg>
          </div>
          <h2>Drop your CSV file here</h2>
          <p>or click to browse — EmpID, ProjectID, DateFrom, DateTo</p>
          {fileName && <span className="fname">📄 {fileName}</span>}
        </div>

        {/* States */}
        {loading && (
          <div className="loading">
            <div className="spinner" /> Analyzing employee records…
          </div>
        )}
        {error && <div className="error">⚠ {error}</div>}

        {/* Result */}
        {result && (
          <div className="result">
            {/* Summary */}
            <div className="summary-card">
              <div className="summary-badge">
                <div className="label">Employee #1</div>
                <div className="value">{result.emp1}</div>
              </div>
              <div className="summary-sep">×</div>
              <div className="summary-badge">
                <div className="label">Employee #2</div>
                <div className="value">{result.emp2}</div>
              </div>
              <div className="total-pill">
                <div className="label">Total Days Together</div>
                <div className="value">{result.totalDays}</div>
              </div>
            </div>

            {/* Projects table */}
            <div className="table-title">Common Projects</div>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Employee ID #1</th>
                    <th>Employee ID #2</th>
                    <th>Project ID</th>
                    <th>Days Worked</th>
                  </tr>
                </thead>
                <tbody>
                  {result.projects
                    .sort((a, b) => b.days - a.days)
                    .map((p, i) => (
                      <tr key={i}>
                        <td><span className="badge-emp">{result.emp1}</span></td>
                        <td><span className="badge-emp">{result.emp2}</span></td>
                        <td>{p.projectId}</td>
                        <td className="days">{p.days}</td>
                      </tr>
                    ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </>
  );
}
