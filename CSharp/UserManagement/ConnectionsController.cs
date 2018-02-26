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

            var combinedRes =
                userRes.Match(new UserLookupResultParameters(otherUserRes));

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

        private class UserLookupResultParameters : 
            IUserLookupResultParameters<ITwoUsersLookupResult>
        {
            private readonly IUserLookupResult otherUserRes;

            public UserLookupResultParameters(IUserLookupResult otherUserRes)
            {
                this.otherUserRes = otherUserRes;
            }

            public ITwoUsersLookupResult OnFound(User user)
            {
                return otherUserRes.Match(
                    new FirstUserFoundLookupResultParameters(user));
            }

            public ITwoUsersLookupResult OnInvalidId
            {
                get { return TwoUsersLookupResult.FirstUserIdInvalid(); }
            }

            public ITwoUsersLookupResult OnNotFound
            {
                get { return TwoUsersLookupResult.FirstUserNotFound(); }
            }
        }

        private class FirstUserFoundLookupResultParameters : 
            IUserLookupResultParameters<ITwoUsersLookupResult>
        {
            private readonly User firstUser;

            public FirstUserFoundLookupResultParameters(User firstUser)
            {
                this.firstUser = firstUser;
            }

            public ITwoUsersLookupResult OnFound(User user)
            {
                return TwoUsersLookupResult.BothFound(firstUser, user);
            }

            public ITwoUsersLookupResult OnInvalidId
            {
                get { return TwoUsersLookupResult.SecondUserIdInvalid(); }
            }

            public ITwoUsersLookupResult OnNotFound
            {
                get { return TwoUsersLookupResult.SecondUserNotFound(); }
            }
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
