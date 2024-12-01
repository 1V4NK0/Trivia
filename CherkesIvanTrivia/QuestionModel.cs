using System;
using System.Collections.Generic;

namespace CherkesIvanTrivia
{
    public class QuestionModel
    {
        // Properties should be declared at the class level, not inside the constructor
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }

        // Constructor to initialize the List property to avoid null reference
        public QuestionModel()
        {
            IncorrectAnswers = new List<string>();
        }
    }
}
