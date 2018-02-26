using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IUserLookupResult<S, E>
    {
        TResult Accept<TResult>(IUserLookupResultVisitor<S, E, TResult> visitor);
    }
}
