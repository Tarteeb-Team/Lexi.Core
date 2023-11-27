//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class NullSpeechException : Xeption
    {
        public NullSpeechException()
            : base(message: "Speech is Null")
        { }
    }
}
