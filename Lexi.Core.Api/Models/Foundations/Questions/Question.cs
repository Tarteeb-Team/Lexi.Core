using System;

namespace Lexi.Core.Api.Models.Foundations.Questions
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Number { get; set; }
        public string QuestionType { get; set; }
    }
}
