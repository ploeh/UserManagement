using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class UserLookupResult
    {
        public static IUserLookupResult<S, E> Success<S, E>(S success)
        {
            return new SuccessUserLookupResult<S, E>(success);
        }

        public static IUserLookupResult<S, E> Error<S, E>(E error)
        {
            return new ErrorUserLookupResult<S, E>(error);
        }
    }
}
