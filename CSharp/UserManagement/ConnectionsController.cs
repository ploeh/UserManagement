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
            IUserLookupResultVisitor<User, ITwoUsersLookupResult<Tuple<User, User>>>
        {
            private readonly IUserLookupResult<User> otherUserRes;

            public UserLookupResultVisitor(IUserLookupResult<User> otherUserRes)
            {
                this.otherUserRes = otherUserRes;
            }

            public ITwoUsersLookupResult<Tuple<User, User>> VisitSuccess(User user)
            {
                return otherUserRes.Accept(
                    new FirstUserFoundLookupResultVisitor(user));
            }

            public ITwoUsersLookupResult<Tuple<User, User>> VisitInvalidId
            {
                get { return TwoUsersLookupResult.FirstUserIdInvalid(); }
            }

            public ITwoUsersLookupResult<Tuple<User, User>> VisitNotFound
            {
                get { return TwoUsersLookupResult.FirstUserNotFound(); }
            }
        }

        private class FirstUserFoundLookupResultVisitor :
            IUserLookupResultVisitor<User, ITwoUsersLookupResult<Tuple<User, User>>>
        {
            private readonly User firstUser;

            public FirstUserFoundLookupResultVisitor(User firstUser)
            {
                this.firstUser = firstUser;
            }

            public ITwoUsersLookupResult<Tuple<User, User>> VisitSuccess(User user)
            {
                return TwoUsersLookupResult.Success(Tuple.Create(firstUser, user));
            }

            public ITwoUsersLookupResult<Tuple<User, User>> VisitInvalidId
            {
                get { return TwoUsersLookupResult.SecondUserIdInvalid(); }
            }

            public ITwoUsersLookupResult<Tuple<User, User>> VisitNotFound
            {
                get { return TwoUsersLookupResult.SecondUserNotFound(); }
            }
        }

        private class TwoUsersLookupToHttpVisitor :
            ITwoUsersLookupResultVisitor<Tuple<User, User>, IHttpActionResult>
        {
            private readonly ConnectionsController controller;

            public TwoUsersLookupToHttpVisitor(ConnectionsController controller)
            {
                this.controller = controller;
            }

            public IHttpActionResult VisitSuccess(Tuple<User, User> t)
            {
                var user = t.Item1;
                var otherUser = t.Item2;

                user.Connect(otherUser);
                controller.UserRepository.Update(user);

                return controller.Ok(otherUser);
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

        private IUserLookupResult<User> LookupUser(string id)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return UserLookupResult.Success(user);

            int userInt;
            if (!int.TryParse(id, out userInt))
                return UserLookupResult.InvalidUserId();

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return UserLookupResult.UserNotFound();

            return UserLookupResult.Success(user);
        }
    }
}
