//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Users;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<User> InsertUserAsync(User user);
        ValueTask<User> UpdateUserAsync(User user);
        ValueTask<User> SelectUserById(Guid id);
    }
}
