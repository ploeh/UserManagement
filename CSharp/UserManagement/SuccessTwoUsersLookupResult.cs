using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class SuccessTwoUsersLookupResult<S> : ITwoUsersLookupResult<S>
    {
        private readonly S success;

        public SuccessTwoUsersLookupResult(S success)
        {
            this.success = success;
        }

        public TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<S, TResult> visitor)
        {
            return visitor.VisitSuccess(success);
        }
    }
}
