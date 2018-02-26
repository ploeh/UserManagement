using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class FoundUserLookupResult : IUserLookupResult
    {
        private readonly User user;

        public FoundUserLookupResult(User user)
        {
            this.user = user;
        }

        public TResult Accept<TResult>(IUserLookupResultVisitor<TResult> visitor)
        {
            return visitor.VisitFound(user);
        }
    }
}
