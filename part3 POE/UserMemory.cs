using System;
using System.Collections.Generic;

namespace MyPOE
{

    public class UserMemory
    {
        private Dictionary<string, string> _facts = new Dictionary<string, string>();

        public Action<string, string>? OnMemoryUpdated;


        public void Remember(string key, string value)
        {
            _facts[key.ToLower()] = value;
            OnMemoryUpdated?.Invoke(key, value);
        }


        public string? Recall(string key)
        {
            return _facts.TryGetValue(key.ToLower(), out string? value) ? value : null;
        }


        public Dictionary<string, string> GetAllFacts()
        {
            return new Dictionary<string, string>(_facts);
        }


        public void ScanAndRemember(string lowerInput, string originalInput)
        {
            string[] interestPhrases = {
                "i'm interested in", "i am interested in",
                "i like", "i want to learn about",
                "my favourite topic is", "my favorite topic is"
            };

            foreach (string phrase in interestPhrases)
            {
                if (lowerInput.Contains(phrase))
                {
                    int startIndex = lowerInput.IndexOf(phrase) + phrase.Length;
                    string interest = originalInput.Substring(startIndex).Trim().TrimEnd('.', '!', '?');
                    if (!string.IsNullOrWhiteSpace(interest))
                    {
                        Remember("interest", interest);
                    }
                    break;
                }
            }

            string[] namePhrases = { "my name is", "call me", "i'm called" };
            foreach (string phrase in namePhrases)
            {
                if (lowerInput.Contains(phrase))
                {
                    int startIndex = lowerInput.IndexOf(phrase) + phrase.Length;
                    string name = originalInput.Substring(startIndex).Trim().TrimEnd('.', '!', '?');
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        Remember("name", name);
                    }
                    break;
                }
            }
        }
    }
}
