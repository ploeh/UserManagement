using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public class User
    {
        public User(int id)
        {
            this.Id = id;
        }

        public int Id { get; }
        public string Email { get; set; }

        public void Connect(User otherUser)
        {
        }
    }
}
