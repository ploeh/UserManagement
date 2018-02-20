using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public class ConnectionsController : ApiController
    {
        public ConnectionsController(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public IUserRepository UserRepository { get; }

        public IHttpActionResult Post(string userId, string otherUserId)
        {
            var otherUser = UserRepository.ReadUser(int.Parse(otherUserId));
            return Ok(otherUser);
        }
    }
}
