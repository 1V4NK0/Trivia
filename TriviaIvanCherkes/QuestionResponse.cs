﻿using System;
namespace TriviaIvanCherkes
{
	public class QuestionResponse
	{
        public int ResponseCode { get; set; }
        public List<Question> Results { get; set; }
    }
}
