using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class FirstInvalidIdTwoUsersLookupResult : ITwoUsersLookupResult
    {
        public TResult Match<TResult>(
            Func<User, User, TResult> onBothFound,
            TResult onFirstInvalidId,
            TResult onFirstNotFound,
            TResult onSecondInvalidId,
            TResult onSecondNotFound)
        {
            return onFirstInvalidId;
        }
    }
}
