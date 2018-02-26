using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class InvalidIdUserLookupResult<S> : IUserLookupResult<S>
    {
        public TResult Accept<TResult>(
            IUserLookupResultVisitor<S, TResult> visitor)
        {
            return visitor.VisitInvalidId;
        }
    }
}
