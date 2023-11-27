using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class UserAlreadyExistsException : Xeption
    {
        public UserAlreadyExistsException(Exception innerException)
        : base(message: "User already exists", innerException)
        { }
    }
}
