using System;
using System.Collections.Generic;
using System.Linq;

namespace MyPOE
{

    public class ChatEngine
    {
        private UserMemory _memory;
        private SentimentDetector _sentiment;
        private KeywordResponder _responder;
        private ConversationTracker _conversation;
        private Random _random;


        private DatabaseManager _dbManager;
        private QuizEngine _quizEngine;
        private ActivityLogger _activityLogger;
        private NlpProcessor _nlpProcessor;

        public Func<string, string> ProcessInput { get; private set; }
        public Action<string, string>? OnMemoryUpdated;

        private enum ConversationState { AskingName, AskingAge, Chatting, Quiz }
        private ConversationState _state;

        private bool _awaitingReminderConfirmation = false;
        private string _pendingTaskDescription = "";

        public ChatEngine()
        {
            _memory = new UserMemory();
            _sentiment = new SentimentDetector();
            _responder = new KeywordResponder();
            _conversation = new ConversationTracker();
            _random = new Random();

            _dbManager = new DatabaseManager();
            _quizEngine = new QuizEngine();
            _activityLogger = new ActivityLogger();
            _nlpProcessor = new NlpProcessor();

            _state = ConversationState.AskingName;
            ProcessInput = GetResponse;
            _memory.OnMemoryUpdated = (key, value) => OnMemoryUpdated?.Invoke(key, value);

            _activityLogger.LogAction("ChatBot initialized and ready.");
        }

        public string GetResponse(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return "Please type something so I can help you!";

                input = input.Trim();

                switch (_state)
                {
                    case ConversationState.AskingName:
                        return HandleNameInput(input);
                    case ConversationState.AskingAge:
                        return HandleAgeInput(input);
                    case ConversationState.Quiz:
                        return HandleQuizInput(input);
                    case ConversationState.Chatting:
                        return ProcessChatInput(input);
                    default:
                        return "Something went wrong. Please try again.";
                }
            }
            catch (Exception ex)
            {
                _activityLogger.LogAction($"Error: {ex.Message}");
                return "Oops! Something unexpected happened. Could you try rephrasing that?";
            }
        }

        private string HandleNameInput(string input)
        {
            string name = string.IsNullOrWhiteSpace(input) ? "Guest" : input;
            _memory.Remember("name", name);
            _state = ConversationState.AskingAge;
            _activityLogger.LogAction($"User provided name: {name}");
            return $"Nice to meet you, {name}! \n\nHow old are you?";
        }

        private string HandleAgeInput(string input)
        {
            if (int.TryParse(input.Trim(), out int age) && age > 0 && age < 150)
            {
                _memory.Remember("age", age.ToString());
                _state = ConversationState.Chatting;
                string name = _memory.Recall("name") ?? "Guest";
                _activityLogger.LogAction($"User provided age: {age}");

                string greeting = age < 18
                    ? $" Stay safe online, {name}! Always be cautious with your personal information.\n\n"
                    : $"Welcome aboard, {name}! \n\n";

                return greeting + "What would you like to do?\n" +
                       "- Ask about cybersecurity (e.g., Phishing, 2FA)\n" +
                       "- Add a task (e.g., 'Add a task to review privacy settings')\n" +
                       "- Start the Quiz (e.g., 'Start quiz')\n" +
                       "- View my activity log (e.g., 'Show activity log')";
            }

            return "Please enter a valid age (a number between 1 and 149).";
        }

        private string HandleQuizInput(string input)
        {
            string response = _quizEngine.ProcessAnswer(input);
            if (!_quizEngine.IsActive)
            {
                _state = ConversationState.Chatting;
                _activityLogger.LogAction("User completed the quiz.");
            }
            return response;
        }

        private string ProcessChatInput(string input)
        {
            string lowerInput = input.ToLower().Trim();

            if (_awaitingReminderConfirmation)
            {
                _awaitingReminderConfirmation = false;
                if (lowerInput == "yes" || lowerInput.Contains("yes"))
                {
                    DateTime reminderDate = DateTime.Now.AddDays(1);
                    if (lowerInput.Contains("days") && int.TryParse(new string(lowerInput.Where(char.IsDigit).ToArray()), out int days))
                    {
                        reminderDate = DateTime.Now.AddDays(days);
                    }

                    _dbManager.AddTask("Cyber Task", _pendingTaskDescription, reminderDate);
                    _activityLogger.LogAction($"Task added with reminder: '{_pendingTaskDescription}'");
                    return $"Got it! I'll remind you on {reminderDate.ToShortDateString()}.";
                }
                else
                {
                    _dbManager.AddTask("Cyber Task", _pendingTaskDescription, null);
                    _activityLogger.LogAction($"Task added without reminder: '{_pendingTaskDescription}'");
                    return "No problem! The task has been added without a reminder.";
                }
            }

            var nlpResult = _nlpProcessor.ProcessInput(input);
            switch (nlpResult.Intent)
            {
                case UserIntent.StartQuiz:
                    _state = ConversationState.Quiz;
                    _activityLogger.LogAction("User started the quiz.");
                    return _quizEngine.StartQuiz();

                case UserIntent.ViewActivityLog:
                    return _activityLogger.GetLogSummary();

                case UserIntent.ViewTasks:
                    var tasks = _dbManager.GetTasks();
                    _activityLogger.LogAction("User viewed their tasks.");
                    if (tasks.Count == 0) return "You currently have no tasks. You can say 'Add a task...' to create one.";
                    string taskStr = "Here are your tasks:\n\n";
                    foreach (var t in tasks)
                    {
                        string status = t.IsCompleted ? "[Done]" : "[Pending]";
                        string reminder = t.ReminderDate.HasValue ? $"(Reminder: {t.ReminderDate.Value.ToShortDateString()})" : "";
                        taskStr += $"{status} {t.Id}. {(string.IsNullOrEmpty(t.Description) ? "Task" : t.Description)} {reminder}\n";
                    }
                    return taskStr.Trim();

                case UserIntent.AddTask:
                    if (!string.IsNullOrEmpty(nlpResult.ExtractedData))
                    {
                        if (nlpResult.ExtractedDate.HasValue)
                        {
                            _dbManager.AddTask("Cyber Task", nlpResult.ExtractedData, nlpResult.ExtractedDate);
                            _activityLogger.LogAction($"Task added with specific reminder: '{nlpResult.ExtractedData}'");
                            return $"Task added: '{nlpResult.ExtractedData}'. Reminder set for {nlpResult.ExtractedDate.Value.ToShortDateString()}.";
                        }
                        else
                        {
                            _pendingTaskDescription = nlpResult.ExtractedData;
                            _awaitingReminderConfirmation = true;
                            return $"Task added with the description \"{_pendingTaskDescription}\". Would you like a reminder?";
                        }
                    }
                    return "What task would you like me to add? For example, say 'Add a task to update my password'.";
            }

            _memory.ScanAndRemember(lowerInput, input);

            Sentiment sentiment = _sentiment.Detect(lowerInput);
            string sentimentPrefix = _sentiment.GetSentimentResponse(sentiment);

            if (_conversation.IsFollowUp(lowerInput))
            {
                string lastTopic = _conversation.GetLastTopic();
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    var (index, followUpResponse) = _responder.GetResponseWithIndex(lastTopic, _conversation.GetUsedIndices());
                    _conversation.TrackUsedIndex(index);
                    string response = !string.IsNullOrEmpty(sentimentPrefix) ? $"{sentimentPrefix}\n\n{followUpResponse}" : $"Here's another tip on {FormatTopicName(lastTopic)}:\n\n{followUpResponse}";
                    _activityLogger.LogAction($"Provided follow-up on topic: {lastTopic}");
                    return AddPersonalization(response);
                }
                return "I'd love to tell you more, but we haven't discussed a specific topic yet! \n\nTry asking about: Password Safety, Phishing, 2FA, Viruses, Scams, or Privacy!";
            }

            string? matchedKeyword = _responder.FindKeyword(lowerInput);
            if (matchedKeyword != null)
            {
                _conversation.SetTopic(matchedKeyword);
                var (index, keywordResponse) = _responder.GetResponseWithIndex(matchedKeyword);
                _conversation.TrackUsedIndex(index);
                _activityLogger.LogAction($"Responded to topic: {matchedKeyword}");

                if (sentiment != Sentiment.Neutral)
                {
                    return AddPersonalization($"{sentimentPrefix}\n\n{keywordResponse}");
                }
                return AddPersonalization(keywordResponse);
            }

            if (lowerInput.Contains("interested in") || lowerInput.Contains("i like") ||
                lowerInput.Contains("favourite") || lowerInput.Contains("favorite"))
            {
                string? interest = _memory.Recall("interest");
                if (!string.IsNullOrEmpty(interest))
                {
                    string? matchedInterest = _responder.FindKeyword(interest.ToLower());
                    if (matchedInterest != null)
                    {
                        _conversation.SetTopic(matchedInterest);
                        var (index, resp) = _responder.GetResponseWithIndex(matchedInterest);
                        _conversation.TrackUsedIndex(index);
                        return $"Great! I'll remember that you're interested in {interest}. It's a crucial part of staying safe online! \n\n{resp}";
                    }
                    return $"Great! I'll remember that you're interested in {interest}. It's a crucial part of staying safe online! \n\nFeel free to ask me about password safety, phishing, 2FA, viruses, scams, or privacy!";
                }
            }

            if (!string.IsNullOrEmpty(sentimentPrefix) && sentiment != Sentiment.Neutral)
            {
                return $"{sentimentPrefix}\n\nI'm here to help with cybersecurity topics! You can ask me about password safety, phishing, two-factor authentication, viruses, scams, or privacy.";
            }

            return " I'm not sure I understand. Can you try rephrasing?\n\n" +
                   "You can ask me about: Password Safety, Phishing, Two-Factor Authentication (2FA), " +
                   "Viruses & Malware, Scams, or Privacy!\n" +
                   "Or you can say 'Add a task' or 'Start quiz'.";
        }

        private string AddPersonalization(string response)
        {
            string? interest = _memory.Recall("interest");
            if (!string.IsNullOrEmpty(interest) && _random.Next(4) == 0)
            {
                string lastTopic = _conversation.GetLastTopic();
                if (!string.IsNullOrEmpty(lastTopic) && !lastTopic.Equals(interest, StringComparison.OrdinalIgnoreCase))
                {
                    response += $"\n\n As someone interested in {interest}, you might also want to explore how this connects to your area of interest!";
                }
            }
            return response;
        }

        public string GetInitialMessage()
        {
            return "Hello! I'm CyberBot — your Cybersecurity Awareness Assistant! 🛡️\n\n" +
                   "I'm here to help you stay safe online. Let's get started!\n\n" +
                   "What's your name?";
        }

        public UserMemory GetMemory()
        {
            return _memory;
        }

        public List<string> GetTopics()
        {
            return _responder.GetTopics();
        }

        private string FormatTopicName(string topic)
        {
            if (string.IsNullOrEmpty(topic)) return topic;
            return char.ToUpper(topic[0]) + topic.Substring(1);
        }
    }
}
