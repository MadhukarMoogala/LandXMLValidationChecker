# LandXMLValidationChecker 

 Autodesk Civil 3D DA has practical limits on the size and structure of LandXML files it can reliably handle. This tool helps you pre-validate your LandXML files before uploading or using them with Civil 3D DA workflows.
---

## ⚙️ Features

- Parses LandXML and identifies surface groups and surfaces
- Counts number of points (`<P>` elements) per surface
- Displays a **summary** of surface groups, surface counts, and point counts
- Performs **limit checks** to ensure the file adheres to safe processing thresholds

---

## ✅ Theoretical Limits Checked

| Check                            | Threshold         |
|----------------------------------|-------------------|
| Total Points Across All Surfaces | 160 million       |
| Total Number of Surfaces         | 850               |
| Total Points Per Surface Group   | 16 million        |

If any of these limits are exceeded, the tool will emit a warning and stop further checks.

---

📦 Usage
```bash

LandXMLValidationChecker.exe <input.xml> [--summary|-s] [--check|-c]
Arguments
<input.xml>: Path to your LandXML file

--summary or -s: Print a surface summary

--check or -c: Run the limit checks
```
🔍 Sample Output

```bash
=== Surface Summary ===

Group: ExistingGround
  Number of Surfaces: 583
  Total Points:        178469

Group: FinishedGrade
  Number of Surfaces: 1
  Total Points:        1854

Group: SubBase
  Number of Surfaces: 275
  Total Points:        14567

Group: SubGrade
  Number of Surfaces: 24
  Total Points:        4427

----------------------------
Total Surfaces: 883
Total Points:   199317

=== Limit Checks ===
✔ Total Points: ✅ OK (199317 ≤ 160000000)
✔ Total Surfaces: ❌ Exceeded! (883 > 850)
✔ ExistingGround: ✅ OK (178469 ≤ 16000000)
✔ FinishedGrade: ✅ OK (1854 ≤ 16000000)
✔ SubBase: ✅ OK (14567 ≤ 16000000)
✔ SubGrade: ✅ OK (4427 ≤ 16000000)

All limits checked.
```
📎 Notes
The tool assumes a standard LandXML structure with `<Surfaces>`, `<Surface>`, and `<P>` elements.
It does not validate XML schema compliance.
If any limits are exceeded, Civil 3D DA may fail to process the file or return unstable results.

🏗️ Why This Exists
Civil 3D Design Automation has known constraints around LandXML file sizes and complexity. Instead of encountering failure after upload or wasting quota/time on jobs that will not run, this tool lets you validate early and locally.


Madhukar Moogala <br>
APS Developer Advocate <br>
Autodesk Platform Services

