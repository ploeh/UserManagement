using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class FirstInvalidIdTwoUsersLookupResult : ITwoUsersLookupResult
    {
        public TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<TResult> visitor)
        {
            return visitor.VisitFirstInvalidId;
        }
    }
}
