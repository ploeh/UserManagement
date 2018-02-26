using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class SuccessTwoUsersLookupResult<S, E> : ITwoUsersLookupResult<S, E>
    {
        private readonly S success;

        public SuccessTwoUsersLookupResult(S success)
        {
            this.success = success;
        }

        public TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<S, E, TResult> visitor)
        {
            return visitor.VisitSuccess(success);
        }
    }
}
