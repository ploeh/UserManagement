using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IResultVisitor<S, E, TResult>
    {
        TResult VisitSuccess(S success);
        TResult VisitError(E error);
    }
}
