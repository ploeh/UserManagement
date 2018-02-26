using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IUserLookupResultParameters<TResult>
    {
        TResult OnFound(User user);
        TResult OnInvalidId { get; }
        TResult OnNotFound { get; }
    }
}
