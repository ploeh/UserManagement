using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public class ConnectionsController : ApiController
    {
        public ConnectionsController(
            IUserCache userCache,
            IUserRepository userRepository)
        {
            UserCache = userCache;
            UserRepository = userRepository;
        }

        public IUserCache UserCache { get; }

        public IUserRepository UserRepository { get; }

        public IHttpActionResult Post(string userId, string otherUserId)
        {
            var user = UserCache.Find(userId);
            if (user == null)
            {
                user = UserRepository.ReadUser(int.Parse(userId));
            }
            UserRepository.Update(user);

            var otherUser = UserRepository.ReadUser(int.Parse(otherUserId));
            user.Connect(otherUser);
            return Ok(otherUser);
        }
    }
}
