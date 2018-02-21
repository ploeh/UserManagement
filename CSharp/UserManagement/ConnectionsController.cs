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
            User user = null;
            var res = LookupUser(userId);
            switch (res.Status)
            {
                case UserLookupStatus.Found:
                    user = res.User;
                    break;
                case UserLookupStatus.NotFound:
                    return BadRequest("User not found.");
                case UserLookupStatus.InvalidId:
                    return BadRequest("Invalid user ID.");
            }

            User otherUser = null;
            res = LookupUser(otherUserId);
            switch (res.Status)
            {
                case UserLookupStatus.Found:
                    otherUser = res.User;
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

        public class UserLookupResult
        {
            public UserLookupResult(UserLookupStatus status)
            {
                Status = status;
            }

            public UserLookupResult(UserLookupStatus status, User user)
            {
                Status = status;
                User = user;
            }

            public UserLookupStatus Status { get; }
            public User User { get; }
        }

        private UserLookupResult LookupUser(string id)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return new UserLookupResult(UserLookupStatus.Found, user);

            int userInt;
            if (!int.TryParse(id, out userInt))
                return new UserLookupResult(UserLookupStatus.InvalidId);

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return new UserLookupResult(UserLookupStatus.NotFound);

            return new UserLookupResult(UserLookupStatus.Found, user);
        }
    }
}
