# Indexr

A lightweight desktop application for generating formatted document indexes from your project folders.

> ⚠️ **Indexr is currently in active development and is not yet available for download. Watch or star the repo to be notified when it releases.**

---

## What is Indexr?

Indexr scans a project folder and automatically generates a clean, formatted document index — complete with hyperlinks to files and folders, version tracking, and last modified dates. No Word template required.

Built for project managers, developers, and anyone who needs to maintain structured documentation indexes without the manual effort.

---

## Features (v1)

- 📁 Recursive folder scanning with configurable exclusion rules
- 📌 Reference folders without scanning their contents (e.g. Superseded folders)
- 🔗 Hyperlinks to files and folders including network drive UNC paths
- 📅 Last modified date for each file (`yyyy/MM/dd`)
- 💾 Save and reload project settings as `.indexr` files
- 🔢 Automatic version number incrementing on project load
- 🕓 Recent projects list for quick access
- 📄 Clean generated document output — no Word installation required

---

## Tech Stack

- **Framework:** .NET MAUI (Windows first, cross platform ready)
- **Language:** C#
- **Document Generation:** QuestPDF
- **Target Framework:** .NET 10

---

## Status

| Phase | Description | Status |
|---|---|---|
| 1 | Solution setup | ✅ Complete |
| 2 | Core models and enums | ✅ Complete |
| 3 | Core services | ✅ Complete |
| 4 | Document generation | ✅ Complete |
| 5 | MAUI UI | ✅ Complete |
| 6 | Wiring up | ✅ Complete |
| 7 | Polish and testing | 🔲 In-progress |
| 8 | Release | 🔲 Pending |

---

## Roadmap

**V2 — Format Templates**
Load an existing Word or PDF document as a formatting reference. Indexr will extract your organisation's headers, fonts and colour scheme and apply them to the generated output — perfect for branded documentation.

**V3+**
- Multiple output formats

---

## Licence

MIT — free to use, modify and distribute. See [LICENSE](LICENSE) for details.

---

## Support

If you find Indexr useful once released, consider supporting development via [GitHub Sponsors](https://github.com/sponsors/Collins-R) ☕
