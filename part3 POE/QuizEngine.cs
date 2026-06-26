using System;
using System.Collections.Generic;

namespace MyPOE
{
    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; } = "";
    }

    public class QuizEngine
    {
        private List<QuizQuestion> _questions;
        private int _currentQuestionIndex = 0;
        private int _score = 0;
        private bool _isActive = false;

        public bool IsActive => _isActive;

        public QuizEngine()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                    CorrectOptionIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others."
                },
                new QuizQuestion
                {
                    Question = "Which of the following is an example of a strong password?",
                    Options = new List<string> { "A) password123", "B) admin", "C) T7@qP2!sL9", "D) 12345678" },
                    CorrectOptionIndex = 2,
                    Explanation = "A strong password contains a mix of uppercase, lowercase, numbers, and special characters."
                },
                new QuizQuestion
                {
                    Question = "What does 2FA stand for?",
                    Options = new List<string> { "A) Two-File Access", "B) Two-Factor Authentication", "C) Total Fraud Avoidance", "D) Time-Frequency Analysis" },
                    CorrectOptionIndex = 1,
                    Explanation = "2FA stands for Two-Factor Authentication, which adds an extra layer of security."
                },
                new QuizQuestion
                {
                    Question = "True or False: It's safe to use public Wi-Fi for online banking.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectOptionIndex = 1,
                    Explanation = "False. Public Wi-Fi is often unsecured, making it easy for hackers to intercept data."
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new List<string> { "A) A type of computer virus", "B) A scam to steal personal information via fake emails", "C) A new video game", "D) A secure way to browse" },
                    CorrectOptionIndex = 1,
                    Explanation = "Phishing relies on deception, typically via email, to steal personal info."
                },
                new QuizQuestion
                {
                    Question = "Which icon in a web browser indicates a secure connection?",
                    Options = new List<string> { "A) A red X", "B) A magnifying glass", "C) A padlock", "D) A star" },
                    CorrectOptionIndex = 2,
                    Explanation = "A padlock icon and 'https://' indicate the connection is encrypted."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should update your software regularly.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectOptionIndex = 0,
                    Explanation = "True. Updates often contain vital security patches."
                },
                new QuizQuestion
                {
                    Question = "What is malware?",
                    Options = new List<string> { "A) Hardware that malfunctions", "B) Malicious software designed to cause harm", "C) A brand of antivirus", "D) A secure network protocol" },
                    CorrectOptionIndex = 1,
                    Explanation = "Malware is any software intentionally designed to cause damage to a computer or network."
                },
                new QuizQuestion
                {
                    Question = "If a website asks for your Social Security Number to read an article, what should you do?",
                    Options = new List<string> { "A) Provide it", "B) Enter a fake one", "C) Leave the website immediately", "D) Share it if it's a trusted news site" },
                    CorrectOptionIndex = 2,
                    Explanation = "Never give out sensitive personal info just to access basic content."
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new List<string> { "A) Software that speeds up your PC", "B) Malware that locks your data until you pay", "C) A type of email filter", "D) A password manager" },
                    CorrectOptionIndex = 1,
                    Explanation = "Ransomware holds your files hostage in exchange for money."
                },
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for all accounts is a good strategy.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectOptionIndex = 1,
                    Explanation = "False. If one account is breached, all your other accounts are at risk."
                }
            };
        }

        public string StartQuiz()
        {
            _currentQuestionIndex = 0;
            _score = 0;
            _isActive = true;
            return "Let's start the Cybersecurity Quiz! I'll ask you 11 questions.\n\n" + GetCurrentQuestionText();
        }

        private string GetCurrentQuestionText()
        {
            var q = _questions[_currentQuestionIndex];
            string text = $"Question {_currentQuestionIndex + 1}/{_questions.Count}:\n{q.Question}\n";
            foreach (var opt in q.Options)
            {
                text += $"{opt}\n";
            }
            text += "\nPlease type the letter of your answer (e.g., A, B, C, D).";
            return text;
        }

        public string ProcessAnswer(string input)
        {
            if (!_isActive) return "The quiz is not currently active.";

            var q = _questions[_currentQuestionIndex];
            int selectedIndex = -1;
            string cleanInput = input.Trim().ToUpper();

            if (cleanInput.StartsWith("A")) selectedIndex = 0;
            else if (cleanInput.StartsWith("B")) selectedIndex = 1;
            else if (cleanInput.StartsWith("C")) selectedIndex = 2;
            else if (cleanInput.StartsWith("D")) selectedIndex = 3;

            if (selectedIndex == -1 || selectedIndex >= q.Options.Count)
            {
                return "Please enter a valid option (e.g., A, B, C, D).";
            }

            bool isCorrect = (selectedIndex == q.CorrectOptionIndex);
            if (isCorrect)
            {
                _score++;
            }

            string feedback = isCorrect ? " Correct!" : "+ Incorrect.";
            feedback += $" {q.Explanation}\n\n";

            _currentQuestionIndex++;

            if (_currentQuestionIndex < _questions.Count)
            {
                feedback += GetCurrentQuestionText();
                return feedback;
            }
            else
            {
                _isActive = false;
                feedback += $" Quiz Completed!\nYour final score is: {_score}/{_questions.Count}\n";
                if (_score == _questions.Count)
                    feedback += "Great job! You're a cybersecurity pro!";
                else if (_score >= _questions.Count / 2)
                    feedback += "Good effort! Keep learning to stay safe online.";
                else
                    feedback += "It looks like you could brush up on some cybersecurity topics. Feel free to ask me questions!";

                return feedback;
            }
        }
    }
}
