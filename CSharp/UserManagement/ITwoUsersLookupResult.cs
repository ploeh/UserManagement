using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface ITwoUsersLookupResult<S, E>
    {
        TResult Accept<TResult>(
            ITwoUsersLookupResultVisitor<S, E, TResult> visitor);
    }
}
