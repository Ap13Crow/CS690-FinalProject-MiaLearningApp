# CS690-FinalProject-MiaLearningApp
## Mia Learning App

A cross‑platform .NET 8 console application that helps lifelong learners:

| Area | What you can do |
|------|-----------------|
| **Courses & Modules** | add / view / delete courses, set progress %, organise course *modules* and mark them complete |
| **Notes** | attach notes to courses, list by course, free‑text search |
| **Vocabulary** | add bilingual word pairs with explanations & examples, quiz yourself (mastery tracking) |
| **Resources** | store external links (articles, videos) per course |
| **Quizzes** | create course quizzes with due‑dates; the app reminds you with a console beep |
| **Dashboard** | one‑line summary: course count, average progress, notes, vocab |
| **Global Search** | search across courses + notes + vocab in one shot |

---

## 🚀 Quick Start

### Download the executable

Go to the latest release:
https://github.com/Ap13Crow/CS690-FinalProject-MiaLearningApp/releases
Download the executable called Source code.zip
Unzip it in your local directory

### Running the application

Make sure to install dotnet runtime .NET Runtime
Go inside the directory (CS690-FinalProject-MiaLearningApp-2.0.0) you have unzipped
Run the following command in terminal
dotnet BusShuttle.dll
```bash
# download release, then…
unzip MiaLearningApp-v2.1.0.zip
cd MiaLearningApp

# launch on Windows (self‑contained exe)
publish-win/LearningApp.exe

# launch on macOS/Linux/Windows if .NET 8 runtime is present
dotnet publish/fdd/MiaLearningApp.dll   # runtime list: dotnet --list-runtimes
