﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Users;

namespace Lexi.Core.Api.Services.Foundations.Users
{
    public interface IUserService
    {
        ValueTask<User> RetrieveUserByIdAsync(Guid userId);
    }
}