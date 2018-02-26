using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class TwoUsersLookupResult
    {
        public static ITwoUsersLookupResult BothFound(User user1, User user2)
        {
            return new FoundTwoUsersLookupResult(user1, user2);
        }

        public static ITwoUsersLookupResult FirstUserIdInvalid()
        {
            return new FirstInvalidIdTwoUsersLookupResult();
        }

        public static ITwoUsersLookupResult FirstUserNotFound()
        {
            return new FirstNotFoundTwoUsersLookupResult();
        }

        public static ITwoUsersLookupResult SecondUserIdInvalid()
        {
            return new SecondInvalidIdTwoUsersLookupResult();
        }

        public static ITwoUsersLookupResult SecondUserNotFound()
        {
            return new SecondNotFoundTwoUsersLookupResult();
        }
    }
}
