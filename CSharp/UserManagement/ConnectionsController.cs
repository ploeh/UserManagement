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
            User user;
            try
            {
                user = LookupUser(
                    userId,
                    "Invalid user ID.",
                    "User not found.");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            User otherUser;
            try
            {
                otherUser = LookupUser(
                    otherUserId,
                    "Invalid ID for other user.",
                    "Other user not found.");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            user.Connect(otherUser);
            UserRepository.Update(user);

            return Ok(otherUser);
        }

        private User LookupUser(
            string id,
            string invalidMessage,
            string notFoundMessage)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return user;

            int userInt;
            if (!int.TryParse(id, out userInt))
                throw new ArgumentException(invalidMessage);

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                throw new ArgumentException(notFoundMessage);

            return user;
        }
    }
}
