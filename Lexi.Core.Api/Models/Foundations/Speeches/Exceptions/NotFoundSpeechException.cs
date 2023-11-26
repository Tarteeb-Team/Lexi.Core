//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Speeches.Exceptions
{
    public class NotFoundSpeechException : Xeption
    {
        public NotFoundSpeechException(Guid speechId)
            : base(message: $"Couldn't find team with id: {speechId}.")
        { }
    }
}
