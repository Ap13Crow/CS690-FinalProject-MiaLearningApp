# CS690-FinalProject-MiaLearningApp
<p align="center">
  <img src="docs/images/main-menu.png" alt="Mia Learning App Console Screenshot" width="700">
</p>

# Mia Learning App

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

## ✨ Screenshots

| Main Menu | Course‑Modules | Vocabulary Quiz |
|-----------|----------------|-----------------|
| ![menu](docs/images/main-menu.png) | ![modules](docs/images/modules.png) | ![quiz](docs/images/quiz.png) |

---

## 🚀 Quick Start

```bash
# download release, then…
unzip MiaLearningApp-v2.1.0.zip
cd MiaLearningApp

# launch on Windows (self‑contained exe)
publish-win/LearningApp.exe

# launch on macOS/Linux/Windows if .NET 8 runtime is present
dotnet publish/fdd/MiaLearningApp.dll   # runtime list: dotnet --list-runtimes
