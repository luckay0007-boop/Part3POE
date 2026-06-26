using System;
using System.Collections.Generic;

namespace MyPOE
{

    public class ConversationTracker
    {
        private string _lastTopic = "";
        private List<int> _usedResponseIndices = new List<int>();

        private static readonly string[] FollowUpPhrases = {
            "tell me more", "another tip", "explain more", "give me another",
            "more info", "what else", "continue", "go on", "more details",
            "another one", "keep going", "more", "next tip", "any more",
            "tell me another", "one more", "anything else"
        };


        public void SetTopic(string topic)
        {
            if (_lastTopic != topic)
            {
                _usedResponseIndices.Clear();
            }
            _lastTopic = topic;
        }


        public string GetLastTopic()
        {
            return _lastTopic;
        }


        public bool IsFollowUp(string lowerInput)
        {
            foreach (string phrase in FollowUpPhrases)
            {
                if (lowerInput.Contains(phrase))
                {
                    return true;
                }
            }
            return false;
        }


        public void TrackUsedIndex(int index)
        {
            if (!_usedResponseIndices.Contains(index))
            {
                _usedResponseIndices.Add(index);
            }
        }


        public List<int> GetUsedIndices()
        {
            return new List<int>(_usedResponseIndices);
        }

        public void ClearUsedIndices()
        {
            _usedResponseIndices.Clear();
        }
    }
}
