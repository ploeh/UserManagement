using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class UserLookupResult
    {
        public static IUserLookupResult<S> Success<S>(S success)
        {
            return new SuccessUserLookupResult<S>(success);
        }

        public static IUserLookupResult<User> InvalidUserId()
        {
            return new InvalidIdUserLookupResult<User>();
        }

        public static IUserLookupResult<User> UserNotFound()
        {
            return new NotFoundUserLookupResult<User>();
        }
    }
}
