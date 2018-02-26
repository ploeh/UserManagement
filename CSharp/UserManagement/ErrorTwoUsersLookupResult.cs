using System;

namespace Ploeh.Samples.UserManagement
{
    internal class ErrorTwoUsersLookupResult<S, E> : ITwoUsersLookupResult<S, E>
    {
        private E error;

        public ErrorTwoUsersLookupResult(E error)
        {
            this.error = error;
        }

        public TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<S, E, TResult> visitor)
        {
            return visitor.VisitError(error);
        }
    }
}