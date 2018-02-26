using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IUserLookupResult
    {
        TResult Accept<TResult>(IUserLookupResultVisitor<TResult> visitor);
    }
}
