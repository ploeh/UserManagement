using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class SuccessUserLookupResult<S> : IUserLookupResult<S>
    {
        private readonly S success;

        public SuccessUserLookupResult(S success)
        {
            this.success = success;
        }

        public TResult Accept<TResult>(IUserLookupResultVisitor<S, TResult> visitor)
        {
            return visitor.VisitSuccess(success);
        }
    }
}
