using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface ITwoUsersLookupResult
    {
        TResult Match<TResult>(
            Func<User, User, TResult> onBothFound,
            TResult onFirstInvalidId,
            TResult onFirstNotFound,
            TResult onSecondInvalidId,
            TResult onSecondNotFound);
    }
}
