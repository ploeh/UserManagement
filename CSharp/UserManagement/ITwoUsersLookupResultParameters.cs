using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface ITwoUsersLookupResultParameters<TResult>
    {
        TResult OnBothFound(User user1, User user2);
        TResult OnFirstInvalidId { get; }
        TResult OnFirstNotFound { get; }
        TResult OnSecondInvalidId { get; }
        TResult OnSecondNotFound { get; }
    }
}
