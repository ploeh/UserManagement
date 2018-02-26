using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class TwoUsersLookupResult
    {
        public static ITwoUsersLookupResult<S, E> Success<S, E>(S success)
        {
            return new SuccessTwoUsersLookupResult<S, E>(success);
        }

        public static ITwoUsersLookupResult<S, E> Error<S, E>(E error)
        {
            return new ErrorTwoUsersLookupResult<S, E>(error);
        }
    }
}
