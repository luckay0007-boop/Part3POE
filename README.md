CyberBot  Part 3 POE (Cybersecurity Awareness Chatbot)
A WPF GUI chatbot that teaches basic cybersecurity concepts and provides:
•	Task assistant with persistent task storage (SQL Server LocalDB by default)
•	Interactive quiz mini-game (11 questions, immediate feedback, final score)
•	Simple NLP simulation for intent detection (task, quiz, reminders, activity log)
•	Activity log that records bot actions and can be viewed via chat
•	Basic reminders scheduler with in-app notifications
Requirements
•	.NET 10 SDK / Runtime
•	Visual Studio 2022/2026 (WPF) or dotnet CLI
•	SQL Server LocalDB (installed with Visual Studio) — connection string configurable in DatabaseManager.cs
Quick start (Visual Studio)
1.	Clone the repo.
2.	Open part3 POE.slnx in Visual Studio.
3.	Build and run the solution.
Quick start (dotnet CLI) cd "path/to/repo" dotnet restore "part3 POE.slnx" dotnet build "part3 POE.slnx" --configuration Release Open the solution in Visual Studio to run the WPF app (recommended).
Database notes
•	The app currently uses SQL Server LocalDB via Microsoft.Data.SqlClient.
•	To use MySQL (assignment requirement), replace DatabaseManager implementation and update package references/connection string.
Usage highlights
•	Tasks: Add tasks via chat (e.g., "Add a task to enable 2FA") or view tasks in the left Tasks panel. Use Complete/Delete buttons to manage tasks.
•	Quiz: Type or click to start the quiz (chat command: "Start quiz"). Answer with letters (A/B/C/D).
•	NLP: Natural-language-like commands are recognized with simple string/regex matching.
•	Activity log: Ask "Show activity log" in chat to view actions performed by the bot.
•	Reminders: Stored reminders are checked periodically and posted into the chat when due.
CI
•	GitHub Actions workflow (.github/workflows/ci.yml) builds the solution on push/PR using .NET 10.
Development notes
•	UI elements and behavior are in MainWindow.xaml / MainWindow.xaml.cs.
•	Core logic is in ChatEngine, DatabaseManager, QuizEngine, SentimentDetector, KeywordResponder, ConversationTracker, UserMemory, ActivityLogger.
•	To persist reminder notifications across restarts, add a persistent column in the DB and update DatabaseManager.
Contributing
•	Create a branch, make changes, add tests (if applicable), and open a pull request.
License
•	Add a LICENSE file as needed.
If you want, I can:
•	Convert DatabaseManager to MySQL and update instructions.
•	Add OS toast notifications for reminders.
•	Create a small script to package and publish the repo.
Which change should I prepare next?
