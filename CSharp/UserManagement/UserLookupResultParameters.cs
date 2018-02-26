using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public class UserLookupResultParameters<TResult>
    {
        public UserLookupResultParameters(
            Func<User, TResult> onFound,
            TResult onInvalidId,
            TResult onNotFound)
        {
            OnFound = onFound;
            OnInvalidId = onInvalidId;
            OnNotFound = onNotFound;
        }

        public Func<User, TResult> OnFound { get; }
        public TResult OnInvalidId { get; }
        public TResult OnNotFound { get; }
    }
}
