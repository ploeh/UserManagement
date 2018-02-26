using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class NotFoundUserLookupError : IUserLookupError
    {
        public TResult Accept<TResult>(IUserLookupErrorVisitor<TResult> visitor)
        {
            return visitor.VisitNotFound;
        }
    }
}
