using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class FailedUserServiceException : Xeption
    {
        public FailedUserServiceException(Exception innerException)
        : base(message: "Failed user service error occured,fix the error", innerException)
        { }
    }
}
