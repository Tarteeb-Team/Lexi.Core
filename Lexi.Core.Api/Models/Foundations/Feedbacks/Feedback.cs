using Lexi.Core.Api.Models.Foundations.Speeches;
using System;

namespace Lexi.Core.Api.Models.Foundations.Feedbacks
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public decimal Accuracy { get; set; }
        public decimal Fluency { get; set; }
        public decimal Prosody { get; set; }
        public decimal Complenteness { get; set; }
        public decimal Pronunciation { get; set; }
        public Guid SpeechId { get; set; }
        public Speech Speech { get; set; }
    }
}
