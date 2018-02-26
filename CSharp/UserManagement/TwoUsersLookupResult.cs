using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class TwoUsersLookupResult
    {
        public static ITwoUsersLookupResult<S> Success<S>(S success)
        {
            return new SuccessTwoUsersLookupResult<S>(success);
        }

        public static ITwoUsersLookupResult<Tuple<User, User>> Error<S>(
            string error)
        {
            return new ErrorTwoUsersLookupResult<Tuple<User, User>>(error);
        }
    }
}
