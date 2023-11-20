//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.Foundations.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public IQueryable<User> SelectAllUsersAsync() =>
            SelectAll<User>().AsQueryable();
    }
}
