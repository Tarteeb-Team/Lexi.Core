using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Lexi.Core.Api.Models.Foundations.Users;

namespace Lexi.Core.Api.Brokers.UpdateStorages
{
    public partial interface IUpdateStorageBroker
    {
        ValueTask<User> InsertUserAsync(User user);
        IQueryable<User> SelectAllUsers();
        ValueTask<User> UpdateUserAsync(User user);
        IEnumerable<User> RetrieveUserWithoudReveiw();
        ValueTask<User> SelectUserByIdAsync(Guid id);
        ValueTask<User> DeleteUserAsync(User user);
    }
}
