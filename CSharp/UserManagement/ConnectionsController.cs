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
            object res = LookupUser(userId);
            var foundResult = res as FoundUserLookupResult;
            if (foundResult != null)
                user = foundResult.User;
            if (res is NotFoundUserLookupResult)
                return BadRequest("User not found.");
            if (res is InvalidIdUserLookupResult)
                return BadRequest("Invalid user ID.");

            User otherUser = null;
            res = LookupUser(otherUserId);
            foundResult = res as FoundUserLookupResult;
            if (foundResult != null)
                otherUser = foundResult.User;
            if (res is NotFoundUserLookupResult)
                return BadRequest("Other user not found.");
            if (res is InvalidIdUserLookupResult)
                return BadRequest("Invalid ID for other user.");

            user.Connect(otherUser);
            UserRepository.Update(user);

            return Ok(otherUser);
        }

        public class FoundUserLookupResult
        {
            public FoundUserLookupResult(User user)
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));
                User = user;
            }

            public User User { get; }
        }

        public class InvalidIdUserLookupResult { }

        public class NotFoundUserLookupResult { }

        private object LookupUser(string id)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return new FoundUserLookupResult(user);

            int userInt;
            if (!int.TryParse(id, out userInt))
                return new InvalidIdUserLookupResult();

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return new NotFoundUserLookupResult();

            return new FoundUserLookupResult(user);
        }
    }
}
