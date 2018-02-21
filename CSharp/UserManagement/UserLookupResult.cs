using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class UserLookupResult
    {
        public static IUserLookupResult UserFound(User user)
        {
            return new FoundUserLookupResult(user);
        }

        public static IUserLookupResult InvalidUserId()
        {
            return new InvalidIdUserLookupResult();
        }

        public static IUserLookupResult UserNotFound()
        {
            return new NotFoundUserLookupResult();
        }
    }
}
