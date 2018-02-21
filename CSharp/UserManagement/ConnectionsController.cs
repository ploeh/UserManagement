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
            switch (LookupUser(userId, out user))
            {
                case UserLookupStatus.Found:
                    break;
                case UserLookupStatus.NotFound:
                    return BadRequest("User not found.");
                case UserLookupStatus.InvalidId:
                    return BadRequest("Invalid user ID.");
            }

            User otherUser;
            switch (LookupUser(otherUserId, out otherUser))
            {
                case UserLookupStatus.Found:
                    break;
                case UserLookupStatus.NotFound:
                    return BadRequest("Other user not found.");
                case UserLookupStatus.InvalidId:
                    return BadRequest("Invalid ID for other user.");
            }

            user.Connect(otherUser);
            UserRepository.Update(user);

            return Ok(otherUser);
        }

        public enum UserLookupStatus
        {
            Found = 0,
            NotFound,
            InvalidId
        }

        private UserLookupStatus LookupUser(string id, out User user)
        {
            user = UserCache.Find(id);
            if (user != null)
                return UserLookupStatus.Found;

            int userInt;
            if (!int.TryParse(id, out userInt))
                return UserLookupStatus.InvalidId;

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return UserLookupStatus.NotFound;

            return UserLookupStatus.Found;
        }
    }
}
