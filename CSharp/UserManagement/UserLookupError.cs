using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class UserLookupError
    {
        public static IUserLookupError InvalidId { get; } =
            new InvalidIdUserLookupError();

        public static IUserLookupError NotFound { get; } =
            new NotFoundUserLookupError();

        public static IUserLookupErrorVisitor<TResult> Switch<TResult>(
            TResult onInvalidId,
            TResult onNotFound)
        {
            return new UserLookupErrorSwitch<TResult>(onInvalidId, onNotFound);
        }

        private class UserLookupErrorSwitch<TResult> : IUserLookupErrorVisitor<TResult>
        {
            public UserLookupErrorSwitch(TResult onInvalidId, TResult onNotFound)
            {
                VisitInvalidId = onInvalidId;
                VisitNotFound = onNotFound;
            }

            public TResult VisitInvalidId { get; }

            public TResult VisitNotFound { get; }
        }
    }
}
