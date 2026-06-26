using System;
using System.Text.RegularExpressions;

namespace MyPOE
{
    public enum UserIntent
    {
        Unknown,
        StartQuiz,
        ViewActivityLog,
        ViewTasks,
        AddTask,
        SetReminder
    }

    public class NlpResult
    {
        public UserIntent Intent { get; set; }
        public string ExtractedData { get; set; } = "";
        public DateTime? ExtractedDate { get; set; }
    }

    public class NlpProcessor
    {
        public NlpResult ProcessInput(string input)
        {
            string lowerInput = input.ToLower().Trim();
            var result = new NlpResult { Intent = UserIntent.Unknown };

            if (lowerInput.Contains("quiz") || lowerInput.Contains("mini game") || lowerInput.Contains("start game") || lowerInput.Contains("play a game"))
            {
                result.Intent = UserIntent.StartQuiz;
                return result;
            }

            if (lowerInput.Contains("activity log") || lowerInput.Contains("what have you done") || lowerInput.Contains("recent actions") || lowerInput.Contains("show log"))
            {
                result.Intent = UserIntent.ViewActivityLog;
                return result;
            }

            if (lowerInput.Contains("show tasks") || lowerInput.Contains("view tasks") || lowerInput.Contains("my tasks") || lowerInput.Contains("list tasks"))
            {
                result.Intent = UserIntent.ViewTasks;
                return result;
            }


            if (lowerInput.Contains("add task") || lowerInput.Contains("add a task") || lowerInput.Contains("create task") || lowerInput.Contains("remind me to"))
            {
                result.Intent = UserIntent.AddTask;


                string pattern = @"(?:add a task to|add task to|remind me to)\s+(.+)";
                var match = Regex.Match(lowerInput, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string data = match.Groups[1].Value;
                    if (data.Contains("tomorrow"))
                    {
                        result.ExtractedDate = DateTime.Now.AddDays(1);
                        data = data.Replace("tomorrow", "").Trim();
                    }
                    else if (data.Contains("today"))
                    {
                        result.ExtractedDate = DateTime.Now;
                        data = data.Replace("today", "").Trim();
                    }
                    else if (Regex.IsMatch(data, @"in (\d+) days?"))
                    {
                        var dayMatch = Regex.Match(data, @"in (\d+) days?");
                        if (int.TryParse(dayMatch.Groups[1].Value, out int days))
                        {
                            result.ExtractedDate = DateTime.Now.AddDays(days);
                            data = data.Replace(dayMatch.Value, "").Trim();
                        }
                    }

                    result.ExtractedData = Capitalize(data);
                }
                return result;
            }

            if (lowerInput.Contains("set a reminder") || lowerInput.Contains("set reminder"))
            {
                result.Intent = UserIntent.SetReminder;
                return result;
            }

            return result;
        }

        private string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
