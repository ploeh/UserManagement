using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class NotFoundUserLookupResult : IUserLookupResult
    {
        public TResult Accept<TResult>(IUserLookupResultVisitor<TResult> visitor)
        {
            return visitor.VisitNotFound;
        }
    }
}
