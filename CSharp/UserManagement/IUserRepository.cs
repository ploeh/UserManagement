using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public interface IUserRepository
    {
        User ReadUser(int userId);
        void Update(User user);
    }
}
