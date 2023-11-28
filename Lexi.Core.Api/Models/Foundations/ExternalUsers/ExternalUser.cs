//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;

namespace Lexi.Core.Api.Models.Foundations.ExternalUsers
{
    public class ExternalUser
    {
        public Guid Id { get; set; }
        public long TelegramId { get; set; }
        public string Name { get; set; }
    }
}
