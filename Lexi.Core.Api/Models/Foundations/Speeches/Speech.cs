//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Lexi.Core.Api.Models.Foundations.Users;
using System;

namespace Lexi.Core.Api.Models.Foundations.Speeches
{
    public class Speech
    {
        public Guid Id { get; set; }
        public string Sentence { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Feedback Feedbacks { get; set; }
    }
}
