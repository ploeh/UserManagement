using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public class TwoUsersLookupResultParameters<TResult>
    {
        public TwoUsersLookupResultParameters(
            Func<User, User, TResult> onBothFound,
            TResult onFirstInvalidId,
            TResult onFirstNotFound,
            TResult onSecondInvalidId,
            TResult onSecondNotFound)
        {
            OnBothFound = onBothFound;
            OnFirstInvalidId = onFirstInvalidId;
            OnFirstNotFound = onFirstNotFound;
            OnSecondInvalidId = onSecondInvalidId;
            OnSecondNotFound = onSecondNotFound;
        }

        public Func<User, User, TResult> OnBothFound { get; }
        public TResult OnFirstInvalidId { get; }
        public TResult OnFirstNotFound { get; }
        public TResult OnSecondInvalidId { get; }
        public TResult OnSecondNotFound { get; }
    }
}
