//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Speeches;
using System;
using System.Collections.Generic;

namespace Lexi.Core.Api.Models.Foundations.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public long TelegramId { get; set; }
        public string? TelegramName { get; set; }
        public string Level { get; set; } = "A1";
        public State State { get; set; }
        public string Name { get; set; }
        public string? ImprovedSpeech { get; set; }
        public decimal? Overall { get; set; }
        public List<Speech> Speeches { get; set; }
    }
}
