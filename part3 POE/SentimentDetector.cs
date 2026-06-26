using System;
using System.Collections.Generic;

namespace MyPOE
{

    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Confused,
        Happy
    }


    public class SentimentDetector
    {
        private Dictionary<Sentiment, List<string>> _sentimentKeywords;

        private Dictionary<Sentiment, List<string>> _sentimentResponses;

        private Random _random;

        public SentimentDetector()
        {
            _random = new Random();

            _sentimentKeywords = new Dictionary<Sentiment, List<string>>
            {
                { Sentiment.Worried, new List<string> {
                    "worried", "scared", "afraid", "anxious", "nervous",
                    "concerned", "frightened", "terrified", "uneasy", "fear"
                }},
                { Sentiment.Curious, new List<string> {
                    "curious", "wondering", "interested", "want to know",
                    "tell me", "how does", "what is", "explain", "learn"
                }},
                { Sentiment.Frustrated, new List<string> {
                    "frustrated", "annoyed", "angry", "upset", "hate",
                    "terrible", "awful", "fed up", "sick of", "irritated"
                }},
                { Sentiment.Confused, new List<string> {
                    "confused", "don't understand", "doesn't make sense",
                    "lost", "unclear", "what do you mean", "i don't get", "confusing"
                }},
                { Sentiment.Happy, new List<string> {
                    "happy", "great", "awesome", "thanks", "thank you",
                    "love", "excellent", "amazing", "wonderful", "fantastic"
                }}
            };

            _sentimentResponses = new Dictionary<Sentiment, List<string>>
            {
                { Sentiment.Worried, new List<string> {
                    "It's completely understandable to feel that way. Scammers can be very convincing. Let me share some tips to help you stay safe.",
                    "Don't worry — you're taking the right steps by learning about this. Knowledge is your best defence!",
                    "I understand your concern, and it's good that you're being cautious. Here's what you need to know to stay protected:"
                }},
                { Sentiment.Curious, new List<string> {
                    "Great question! I love your curiosity! Let me explain.",
                    "That's a fantastic thing to be curious about — here's what you should know:",
                    "Wonderful! Curiosity is the first step to staying safe online."
                }},
                { Sentiment.Frustrated, new List<string> {
                    "I understand that can be frustrating. Let me try to help make things clearer and more manageable.",
                    "I hear you — cybersecurity can feel overwhelming sometimes. Let's break it down together, step by step.",
                    "I'm sorry you're feeling that way. Let me simplify things for you — it's easier than you might think!"
                }},
                { Sentiment.Confused, new List<string> {
                    "No worries at all — let me explain that in a simpler way!",
                    "That's a common area of confusion, and you're not alone. Let me clarify!",
                    "Let me break that down for you step by step — it's actually quite straightforward."
                }},
                { Sentiment.Happy, new List<string> {
                    "Glad to hear you're in good spirits!  Let's keep the learning going!",
                    "That's awesome! Your positive attitude will help you stay on top of cybersecurity!",
                    "Great to see your enthusiasm! Here's more to keep you informed and safe:"
                }}
            };
        }


        public Sentiment Detect(string lowerInput)
        {
            foreach (var kvp in _sentimentKeywords)
            {
                foreach (string keyword in kvp.Value)
                {
                    if (lowerInput.Contains(keyword))
                    {
                        return kvp.Key;
                    }
                }
            }
            return Sentiment.Neutral;
        }


        public string GetSentimentResponse(Sentiment sentiment)
        {
            if (sentiment == Sentiment.Neutral)
                return "";

            if (_sentimentResponses.TryGetValue(sentiment, out var responses))
            {
                return responses[_random.Next(responses.Count)];
            }

            return "";
        }
    }
}
