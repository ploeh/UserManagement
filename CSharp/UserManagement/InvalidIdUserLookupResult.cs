using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    internal class InvalidIdUserLookupResult : IUserLookupResult
    {
        public TResult Match<TResult>(
            IUserLookupResultParameters<TResult> parameters)
        {
            return parameters.OnInvalidId;
        }
    }
}
