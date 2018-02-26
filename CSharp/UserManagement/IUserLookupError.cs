using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IUserLookupError
    {
        TResult Accept<TResult>(IUserLookupErrorVisitor<TResult> visitor);
    }
}
