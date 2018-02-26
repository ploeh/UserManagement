using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface ITwoUsersLookupResultVisitor<S, TResult>
    {
        TResult VisitSuccess(S success);
        TResult VisitFirstInvalidId { get; }
        TResult VisitFirstNotFound { get; }
        TResult VisitSecondInvalidId { get; }
        TResult VisitSecondNotFound { get; }
    }
}
