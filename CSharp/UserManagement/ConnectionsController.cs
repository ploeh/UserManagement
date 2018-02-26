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
            var userRes = LookupUser(userId).SelectError(
                error => error.Accept(UserLookupError.Switch(
                    onInvalidId: "Invalid user ID.",
                    onNotFound:  "User not found.")));
            var otherUserRes = LookupUser(otherUserId).SelectError(
                error => error.Accept(UserLookupError.Switch(
                    onInvalidId: "Invalid ID for other user.",
                    onNotFound:  "Other user not found.")));

            var connect =
                userRes.Select(
                    user => otherUserRes.Select(otherUser =>
                    {
                        user.Connect(otherUser);
                        UserRepository.Update(user);

                        return otherUser;
                    }));

            return connect
                .SelectError(error => BadRequest(error))
                .Select(r => r.SelectError(error => BadRequest(error)))
                .Select(r => r.Select(u => Ok(u)))
                .Select(r => r.Accept(new ResultToHttpResultVisitor()))
                .Accept(new ResultToHttpResultVisitor());
        }

        private class ResultToHttpResultVisitor :
            IResultVisitor<IHttpActionResult, IHttpActionResult, IHttpActionResult>
        {
            public IHttpActionResult VisitError(IHttpActionResult error)
            {
                return error;
            }

            public IHttpActionResult VisitSuccess(IHttpActionResult success)
            {
                return success;
            }
        }

        private IResult<User, IUserLookupError> LookupUser(string id)
        {
            var user = UserCache.Find(id);
            if (user != null)
                return Result.Success<User, IUserLookupError>(user);

            int userInt;
            if (!int.TryParse(id, out userInt))
                return Result.Error<User, IUserLookupError>(UserLookupError.InvalidId);

            user = UserRepository.ReadUser(userInt);
            if (user == null)
                return Result.Error<User, IUserLookupError>(UserLookupError.NotFound);

            return Result.Success<User, IUserLookupError>(user);
        }
    }
}
