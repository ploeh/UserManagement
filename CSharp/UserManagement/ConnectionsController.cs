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

            var combinedRes = userRes.Match(
                new UserLookupResultParameters<ITwoUsersLookupResult>(
                    onInvalidId: TwoUsersLookupResult.FirstUserIdInvalid(),
                    onNotFound: TwoUsersLookupResult.FirstUserNotFound(),
                    onFound: user => otherUserRes.Match(
                        new UserLookupResultParameters<ITwoUsersLookupResult>(
                            onInvalidId: TwoUsersLookupResult.SecondUserIdInvalid(),
                            onNotFound: TwoUsersLookupResult.SecondUserNotFound(),
                            onFound: otherUser =>
                                TwoUsersLookupResult.BothFound(user, otherUser)))));

            return combinedRes.Match(
                new TwoUsersLookupResultParameters<IHttpActionResult>(
                    onFirstInvalidId: BadRequest("Invalid user ID."),
                    onFirstNotFound: BadRequest("User not found."),
                    onSecondInvalidId: BadRequest("Invalid ID for other user."),
                    onSecondNotFound: BadRequest("Other user not found."),
                    onBothFound: (user, otherUser) =>
                    {
                        user.Connect(otherUser);
                        UserRepository.Update(user);

                        return Ok(otherUser);
                    }));
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
