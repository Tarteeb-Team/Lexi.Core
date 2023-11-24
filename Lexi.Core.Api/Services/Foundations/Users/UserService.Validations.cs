//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Users;
using Lexi.Core.Api.Models.Foundations.Users.Exceptions;

namespace Lexi.Core.Api.Services.Foundations.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserNotNull(user);
        }

        private static void ValidateUserNotNull(User user)
        {
            if (user == null)
            {
                throw new NullUserException();
            }
        }
    }
}
