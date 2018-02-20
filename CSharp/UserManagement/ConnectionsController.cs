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
                int userInt;
                if (int.TryParse(userId, out userInt))
                {
                    user = UserRepository.ReadUser(userInt);
                    if (user == null)
                        return BadRequest("User not found.");
                }
                else return BadRequest("Invalid user ID.");
            }

            var otherUser = UserCache.Find(otherUserId);
            if (otherUser == null)
            {
                int otherUserInt;
                if (int.TryParse(otherUserId, out otherUserInt))
                {
                    otherUser = UserRepository.ReadUser(otherUserInt);
                }
                else return BadRequest("Invalid user ID for other user.");
            }

            user.Connect(otherUser);
            UserRepository.Update(user);

            return Ok(otherUser);
        }
    }
}
