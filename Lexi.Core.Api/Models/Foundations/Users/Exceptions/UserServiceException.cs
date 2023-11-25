using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class UserServiceException : Xeption
    {
        public UserServiceException(Xeption innerException)
       : base("User service error occured,fix the error", innerException)
        { }
    }
}
