using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class SuccessUserLookupResult<S, E> : IUserLookupResult<S, E>
    {
        private readonly S success;

        public SuccessUserLookupResult(S success)
        {
            this.success = success;
        }

        public TResult Accept<TResult>(IUserLookupResultVisitor<S, E, TResult> visitor)
        {
            return visitor.VisitSuccess(success);
        }
    }
}
