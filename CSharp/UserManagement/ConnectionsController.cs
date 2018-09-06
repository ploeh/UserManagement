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
            IUserReader userReader,
            IUserRepository userRepository)
        {
            UserReader = userReader;
            UserRepository = userRepository;
        }

        public IUserReader UserReader { get; }
        public IUserRepository UserRepository { get; }

        public IHttpActionResult Post(string userId, string otherUserId)
        {
            var userRes = UserReader.Lookup(userId).SelectError(
                error => error.Accept(UserLookupError.Switch(
                    onInvalidId: "Invalid user ID.",
                    onNotFound:  "User not found.")));
            var otherUserRes = UserReader.Lookup(otherUserId).SelectError(
                error => error.Accept(UserLookupError.Switch(
                    onInvalidId: "Invalid ID for other user.",
                    onNotFound:  "Other user not found.")));

            var connect =
                from user in userRes
                from otherUser in otherUserRes
                select Connect(user, otherUser);

            return connect.SelectBoth(Ok, BadRequest).Bifold();
        }

        private User Connect(User user, User otherUser)
        {
            user.Connect(otherUser);
            UserRepository.Update(user);

            return otherUser;
        }
    }
}
