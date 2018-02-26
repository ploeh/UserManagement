using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class InvalidIdUserLookupError : IUserLookupError
    {
        public TResult Accept<TResult>(IUserLookupErrorVisitor<TResult> visitor)
        {
            return visitor.VisitInvalidId;
        }
    }
}
