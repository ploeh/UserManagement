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
                userRes.Accept(new UserLookupResultVisitor(otherUserRes));

            return combinedRes.Accept(new TwoUsersLookupToHttpVisitor(this));
        }

        private class UserLookupResultVisitor : 
            IUserLookupResultVisitor<ITwoUsersLookupResult>
        {
            private readonly IUserLookupResult otherUserRes;

            public UserLookupResultVisitor(IUserLookupResult otherUserRes)
            {
                this.otherUserRes = otherUserRes;
            }

            public ITwoUsersLookupResult VisitFound(User user)
            {
                return otherUserRes.Accept(
                    new FirstUserFoundLookupResultVisitor(user));
            }

            public ITwoUsersLookupResult VisitInvalidId
            {
                get { return TwoUsersLookupResult.FirstUserIdInvalid(); }
            }

            public ITwoUsersLookupResult VisitNotFound
            {
                get { return TwoUsersLookupResult.FirstUserNotFound(); }
            }
        }

        private class FirstUserFoundLookupResultVisitor :
            IUserLookupResultVisitor<ITwoUsersLookupResult>
        {
            private readonly User firstUser;

            public FirstUserFoundLookupResultVisitor(User firstUser)
            {
                this.firstUser = firstUser;
            }

            public ITwoUsersLookupResult VisitFound(User user)
            {
                return TwoUsersLookupResult.BothFound(firstUser, user);
            }

            public ITwoUsersLookupResult VisitInvalidId
            {
                get { return TwoUsersLookupResult.SecondUserIdInvalid(); }
            }

            public ITwoUsersLookupResult VisitNotFound
            {
                get { return TwoUsersLookupResult.SecondUserNotFound(); }
            }
        }

        private class TwoUsersLookupToHttpVisitor :
            ITwoUsersLookupResultVisitor<IHttpActionResult>
        {
            private readonly ConnectionsController controller;

            public TwoUsersLookupToHttpVisitor(ConnectionsController controller)
            {
                this.controller = controller;
            }

            public IHttpActionResult VisitBothFound(User user1, User user2)
            {
                user1.Connect(user2);
                controller.UserRepository.Update(user1);

                return controller.Ok(user2);
            }

            public IHttpActionResult VisitFirstInvalidId
            {
                get { return controller.BadRequest("Invalid user ID."); }
            }

            public IHttpActionResult VisitFirstNotFound
            {
                get { return controller.BadRequest("User not found."); }
            }

            public IHttpActionResult VisitSecondInvalidId
            {
                get { return controller.BadRequest("Invalid ID for other user."); }
            }

            public IHttpActionResult VisitSecondNotFound
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
