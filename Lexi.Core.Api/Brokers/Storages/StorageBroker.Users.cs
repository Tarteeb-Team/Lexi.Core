//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);
        public async ValueTask<User> UpdateUserAsync(User user) =>
            await UpdateAsync(user);

        public IEnumerable<User> RetrieveUserWithoudReveiw()
        {
            var allUsers = SelectAllUsers().ToList();
            var allReviews = SelectAllReviews().ToList();

            var usersWithoutReview = allUsers.Where(u => !allReviews.Any(r => r.TelegramId == u.TelegramId));

            return usersWithoutReview;
        }

        public async ValueTask<User> SelectUserByIdAsync(Guid id) =>
            await SelectAsync<User>(id);
        public IQueryable<User> SelectAllUsers() =>
            SelectAll<User>().AsQueryable();
        public ValueTask<User> DeleteUserAsync(User user) =>
            DeleteAsync(user);
    }
}
