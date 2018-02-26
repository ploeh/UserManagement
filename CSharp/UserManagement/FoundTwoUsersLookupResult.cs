using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class FoundTwoUsersLookupResult : ITwoUsersLookupResult
    {
        private readonly User user1;
        private readonly User user2;

        public FoundTwoUsersLookupResult(User user1, User user2)
        {
            this.user1 = user1;
            this.user2 = user2;
        }

        public TResult Match<TResult>(
            TwoUsersLookupResultParameters<TResult> parameters)
        {
            return parameters.OnBothFound(user1, user2);
        }
    }
}
