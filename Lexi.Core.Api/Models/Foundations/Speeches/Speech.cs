//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;

namespace Lexi.Core.Api.Models.Foundations.Speeches
{
    public class Speech
    {
        public Guid Id { get; set; }
        public string Sentence { get; set; }
        public Guid UserId { get; set; }
    }
}
