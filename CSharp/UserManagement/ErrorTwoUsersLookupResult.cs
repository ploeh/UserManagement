using System;

namespace Ploeh.Samples.UserManagement
{
    internal class ErrorTwoUsersLookupResult<S> : ITwoUsersLookupResult<S>
    {
        private string error;

        public ErrorTwoUsersLookupResult(string error)
        {
            this.error = error;
        }

        public TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<S, TResult> visitor)
        {
            return visitor.VisitError(error);
        }
    }
}