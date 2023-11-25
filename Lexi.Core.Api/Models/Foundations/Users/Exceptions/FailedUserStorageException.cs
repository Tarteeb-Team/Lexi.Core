using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class FailedUserStorageException : Xeption
    {
        public FailedUserStorageException(Exception innerException)
        :base(message:"Failed user storage error occured,fix the error",innerException)
        { }
    }
}
