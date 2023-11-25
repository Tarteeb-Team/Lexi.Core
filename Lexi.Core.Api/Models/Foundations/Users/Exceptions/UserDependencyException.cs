using Xeptions;

namespace Lexi.Core.Api.Models.Foundations.Users.Exceptions
{
    public class UserDependencyException : Xeption
    {
        public UserDependencyException(Xeption innerException) 
        :base(message: "User dependency error ocured.Fix the error and try again",innerException)
        { }
    }
}
