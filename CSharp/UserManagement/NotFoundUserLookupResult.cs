using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class NotFoundUserLookupResult : IUserLookupResult
    {
        public TResult Match<TResult>(
            Func<User, TResult> onFound,
            TResult onInvalidId,
            TResult onNotFound)
        {
            return onNotFound;
        }
    }
}
