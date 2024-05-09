using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Security.Principal;

namespace Lexi.Core.Api.Models.Foundations.QuestionTypes
{
    public class QuestionType
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public string Type { get; set; }
        public long TelegramId { get; set; }
    }
}
