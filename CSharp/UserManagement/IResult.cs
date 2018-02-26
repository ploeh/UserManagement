using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IResult<S, E>
    {
        TResult Accept<TResult>(IResultVisitor<S, E, TResult> visitor);
    }
}
