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
        public string Name { get; set; }
        public List<Speech> Speeches { get; set; }
    }
}
