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
            IUserLookupResultVisitor<User, IUserLookupError, ITwoUsersLookupResult<Tuple<User, User>, string>>
        {
            private readonly IUserLookupResult<User, IUserLookupError> otherUserRes;

            public UserLookupResultVisitor(
                IUserLookupResult<User, IUserLookupError> otherUserRes)
            {
                this.otherUserRes = otherUserRes;
            }

            public ITwoUsersLookupResult<Tuple<User, User>, string> VisitSuccess(
                User user)
            {
                return otherUserRes.Accept(
                    new FirstUserFoundLookupResultVisitor(user));
            }

            public ITwoUsersLookupResult<Tuple<User, User>, string> VisitError(
                IUserLookupError error)
            {
                return TwoUsersLookupResult.Error<Tuple<User, User>, string>(
                    error.Accept(UserLookupError.Switch(
                        onInvalidId: "Invalid user ID.",
                        onNotFound:  "User not found.")));
            }
        }

        private class FirstUserFoundLookupResultVisitor :
            IUserLookupResultVisitor<User, IUserLookupError, ITwoUsersLookupResult<Tuple<User, User>, string>>
        {
            private readonly User firstUser;

            public FirstUserFoundLookupResultVisitor(User firstUser)
            {
                this.firstUser = firstUser;
            }

            public ITwoUsersLookupResult<Tuple<User, User>, string> VisitSuccess(
                User user)
            {
                return TwoUsersLookupResult.Success<Tuple<User, User>, string>(
                    Tuple.Create(firstUser, user));
            }

            public ITwoUsersLookupResult<Tuple<User, User>, string> VisitError(
                IUserLookupError error)
            {
                return TwoUsersLookupResult.Error<Tuple<User, User>, string>(
                    error.Accept(UserLookupError.Switch(
                        onInvalidId: "Invalid ID for other user.",
                        onNotFound:  "Other user not found.")));
            }
        }

        private class TwoUsersLookupToHttpVisitor :
            ITwoUsersLookupResultVisitor<Tuple<User, User>, string, IHttpActionResult>
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

            public IHttpActionResult VisitError(string error)
            {
                return controller.BadRequest(error);
            }
        }

        private IUserLookupResult<User, IUserLookupError> LookupUser(string id)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return UserLookupResult.Success<User, IUserLookupError>(user);

            int userInt;
            if (!int.TryParse(id, out userInt))
                return UserLookupResult.Error<User, IUserLookupError>(UserLookupError.InvalidId);

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return UserLookupResult.Error<User, IUserLookupError>(UserLookupError.NotFound);

            return UserLookupResult.Success<User, IUserLookupError>(user);
        }
    }
}
