using AutoFixture.Xunit2;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.UserManagement.UnitTests
{
    public class ConnectionsControllerTests
    {
        [Theory, UserManagementTestConventions]
        public void UsersSuccessfullyConnect(
            [Frozen]Mock<IUserReader> readerTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            User otherUser,
            ConnectionsController sut)
        {
            readerTD
                .Setup(r => r.Lookup(user.Id.ToString()))
                .Returns(Result.Success<User, IUserLookupError>(user));
            readerTD
                .Setup(r => r.Lookup(otherUser.Id.ToString()))
                .Returns(Result.Success<User, IUserLookupError>(otherUser));

            var actual = sut.Post(user.Id.ToString(), otherUser.Id.ToString());

            var ok = Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(
                actual);
            Assert.Equal(otherUser, ok.Content);
            repoTD.Verify(r => r.Update(user));
            Assert.Contains(otherUser.Id, user.Connections);
        }

        [Theory, UserManagementTestConventions]
        public void UsersFailToConnectWhenUserIdIsInvalid(
            [Frozen]Mock<IUserReader> readerTD,
            [Frozen]Mock<IUserRepository> repoTD,
            string userId,
            User otherUser,
            ConnectionsController sut)
        {
            Assert.False(int.TryParse(userId, out var _));
            readerTD
                .Setup(r => r.Lookup(userId))
                .Returns(Result.Error<User, IUserLookupError>(
                    UserLookupError.InvalidId));
            readerTD
                .Setup(r => r.Lookup(otherUser.Id.ToString()))
                .Returns(Result.Success<User, IUserLookupError>(otherUser));

            var actual = sut.Post(userId, otherUser.Id.ToString());

            var err = Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            Assert.Equal("Invalid user ID.", err.Message);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Theory, UserManagementTestConventions]
        public void UsersFailToConnectWhenOtherUserIdIsInvalid(
            [Frozen]Mock<IUserReader> readerTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            string otherUserId,
            ConnectionsController sut)
        {
            Assert.False(int.TryParse(otherUserId, out var _));
            readerTD
                .Setup(r => r.Lookup(user.Id.ToString()))
                .Returns(Result.Success<User, IUserLookupError>(user));
            readerTD
                .Setup(r => r.Lookup(otherUserId))
                .Returns(Result.Error<User, IUserLookupError>(
                    UserLookupError.InvalidId));

            var actual = sut.Post(user.Id.ToString(), otherUserId);

            var err = Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            Assert.Equal("Invalid ID for other user.", err.Message);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Theory, UserManagementTestConventions]
        public void UsersDoNotConnectWhenUserDoesNotExist(
            [Frozen]Mock<IUserReader> readerTD,
            [Frozen]Mock<IUserRepository> repoTD,
            string userId,
            User otherUser,
            ConnectionsController sut)
        {
            readerTD
                .Setup(r => r.Lookup(userId))
                .Returns(Result.Error<User, IUserLookupError>(
                    UserLookupError.NotFound));
            readerTD
                .Setup(r => r.Lookup(otherUser.Id.ToString()))
                .Returns(Result.Success<User, IUserLookupError>(otherUser));

            var actual = sut.Post(userId, otherUser.Id.ToString());

            var err = Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            Assert.Equal("User not found.", err.Message);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Theory, UserManagementTestConventions]
        public void UsersDoNotConnectWhenOtherUserDoesNotExist(
            [Frozen]Mock<IUserReader> readerTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            int otherUserId,
            ConnectionsController sut)
        {
            readerTD
                .Setup(r => r.Lookup(user.Id.ToString()))
                .Returns(Result.Success<User, IUserLookupError>(user));
            readerTD
                .Setup(r => r.Lookup(otherUserId.ToString()))
                .Returns(Result.Error<User, IUserLookupError>(
                    UserLookupError.NotFound));

            var actual = sut.Post(user.Id.ToString(), otherUserId.ToString());

            var err = Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            Assert.Equal("Other user not found.", err.Message);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }
    }
}
