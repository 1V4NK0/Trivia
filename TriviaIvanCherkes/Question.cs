using System;
namespace TriviaIvanCherkes
{
    public class Question
    {
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public string QuestionText { get; set; }  // Changed property name
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
    }

}

