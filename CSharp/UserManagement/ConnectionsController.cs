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
            var userRes = LookupUser(userId);
            var otherUserRes = LookupUser(otherUserId);

            return userRes.Match(
                onInvalidId: BadRequest("Invalid user ID."),
                onNotFound: BadRequest("User not found."),
                onFound: user =>
                {
                    return otherUserRes.Match<IHttpActionResult>(
                        onInvalidId: BadRequest("Invalid ID for other user."),
                        onNotFound: BadRequest("Other user not found."),
                        onFound: otherUser =>
                        {
                            user.Connect(otherUser);
                            UserRepository.Update(user);
                            return Ok(otherUser);
                        });
                });
        }

        private IUserLookupResult LookupUser(string id)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return UserLookupResult.UserFound(user);

            int userInt;
            if (!int.TryParse(id, out userInt))
                return UserLookupResult.InvalidUserId();

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return UserLookupResult.UserNotFound();

            return UserLookupResult.UserFound(user);
        }
    }
}
