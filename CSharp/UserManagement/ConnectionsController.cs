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

            return combinedRes.Match(new TwoUsersLookupToHttpParameters(this));
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

        private class TwoUsersLookupToHttpParameters : 
            ITwoUsersLookupResultParameters<IHttpActionResult>
        {
            private readonly ConnectionsController controller;

            public TwoUsersLookupToHttpParameters(
                ConnectionsController controller)
            {
                this.controller = controller;
            }

            public IHttpActionResult OnBothFound(User user1, User user2)
            {
                user1.Connect(user2);
                controller.UserRepository.Update(user1);

                return controller.Ok(user2);
            }

            public IHttpActionResult OnFirstInvalidId
            {
                get { return controller.BadRequest("Invalid user ID."); }
            }

            public IHttpActionResult OnFirstNotFound
            {
                get { return controller.BadRequest("User not found."); }
            }

            public IHttpActionResult OnSecondInvalidId
            {
                get { return controller.BadRequest("Invalid ID for other user."); }
            }

            public IHttpActionResult OnSecondNotFound
            {
                get { return controller.BadRequest("Other user not found."); }
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
