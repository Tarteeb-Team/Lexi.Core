using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class UserDependencyValidationException : Xeption
    {
        public UserDependencyValidationException(Exception innerException)
        : base(message: "User dependency validation error ocured.Fix the error and try again", innerException)
        { }
    }
}
