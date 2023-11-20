﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Users;
using Microsoft.EntityFrameworkCore;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);
        public async ValueTask<User> UpdateUserAsync(User user) =>
            await UpdateUserAsync(user);
        public async ValueTask<User> SelectUserById(Guid id) =>
            await SelectAsync<User>(id);
        public IQueryable<User> GetUsers() =>
            SelectAll<User>().AsQueryable();
    }
}
