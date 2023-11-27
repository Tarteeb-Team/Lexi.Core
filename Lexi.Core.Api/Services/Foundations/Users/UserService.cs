//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Speeches;
using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Services.Foundations.Users
{
    public partial class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }
        public ValueTask<User> AddUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUserOnAdd(user);

            return await this.storageBroker.InsertUserAsync(user);
        });

        public async ValueTask<User> ModifyUserAsync(User user)
        {
            User modifiedUser = await this.storageBroker.UpdateUserAsync(user);

            return modifiedUser;
        }

        public  ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
            TryCatch(async () =>
        {
            ValidateUserId(userId);
            User maybeUser = await storageBroker.SelectUserByIdAsync(userId);
            ValidateStorageUser(maybeUser,userId);
            return maybeUser;
        });
        public IQueryable<User> RetrieveAllUsers() =>
        TryCatch(() =>
        {
            return this.storageBroker.SelectAllUsers();
        });

        public  ValueTask<User> RemoveUserAsync(Guid userId) =>
        TryCatch(async () =>
        {
            ValidateUserId(userId);

            User maybeUser =
                await this.storageBroker.SelectUserByIdAsync(userId);

            ValidateStorageUser(maybeUser,userId);

            return await this.storageBroker.DeleteUserAsync(maybeUser);
        });
    }
}
