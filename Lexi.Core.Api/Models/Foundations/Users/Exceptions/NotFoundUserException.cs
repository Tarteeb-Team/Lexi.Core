using System;
using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(Guid id) 
        :base(message:$"Couldn't find user with id: {id}.")
        { }
    }
}
