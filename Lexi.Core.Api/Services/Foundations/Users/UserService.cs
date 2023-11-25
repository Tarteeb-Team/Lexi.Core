//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Brokers.Loggings;
using Lexi.Core.Api.Brokers.Storages;
using Lexi.Core.Api.Models.Foundations.Users;

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
        TryCatch( async () =>
        {
            ValidateUserOnAdd(user);

            return await this.storageBroker.InsertUserAsync(user);
        });

        public async ValueTask<User> ModifyUserAsync(User user)
        {
            User modifiedUser = await this.storageBroker.UpdateUserAsync(user);

            return modifiedUser;
        }

        public async ValueTask<User> RetrieveUserByIdAsync(Guid userId)
        {
            User persistedUser = await this.storageBroker.SelectUserByIdAsync(userId);

            return persistedUser;
        }
        public IQueryable<User> RetrieveAllUsers()
        {
            return this.storageBroker.SelectAllUsers();
        }
        public async ValueTask<User> DeleteUserAsync(User user)
        {
            return await this.storageBroker.DeleteUserAsync(user);
        }
    }
}
