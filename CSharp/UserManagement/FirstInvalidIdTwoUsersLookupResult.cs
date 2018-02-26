using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class FirstInvalidIdTwoUsersLookupResult<S> : ITwoUsersLookupResult<S>
    {
        public TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<S, TResult> visitor)
        {
            return visitor.VisitFirstInvalidId;
        }
    }
}
