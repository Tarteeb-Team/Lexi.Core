using System;

namespace Lexi.Core.Api.Models.Foundations.Reviews
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public long TelegramId { get; set; }
        public string TelegramUserName { get; set; }
    }
}
