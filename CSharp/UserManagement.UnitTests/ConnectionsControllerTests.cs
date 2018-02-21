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
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            User otherUser,
            ConnectionsController sut)
        {
            cacheTD.Setup(c => c.Find(     user.Id.ToString())).Returns((User)null);
            cacheTD.Setup(c => c.Find(otherUser.Id.ToString())).Returns((User)null);
            repoTD.Setup(r => r.ReadUser(user.Id)).Returns(user);
            repoTD.Setup(r => r.ReadUser(otherUser.Id)).Returns(otherUser);

            var actual = sut.Post(user.Id.ToString(), otherUser.Id.ToString());

            var ok = Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(
                actual);
            Assert.Equal(otherUser, ok.Content);
            repoTD.Verify(r => r.Update(user));
            Assert.Contains(otherUser.Id, user.Connections);
        }

        [Theory, UserManagementTestConventions]
        public void UsersSuccessfullyConnectWhenUserIsInCache(
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            User otherUser,
            ConnectionsController sut)
        {
            cacheTD.Setup(c => c.Find(user.Id.ToString())).Returns(user);
            cacheTD.Setup(c => c.Find(otherUser.Id.ToString())).Returns((User)null);
            repoTD.Setup(r => r.ReadUser(otherUser.Id)).Returns(otherUser);

            var actual = sut.Post(user.Id.ToString(), otherUser.Id.ToString());

            var ok = Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(
                actual);
            Assert.Equal(otherUser, ok.Content);
            repoTD.Verify(r => r.Update(user));
            Assert.Contains(otherUser.Id, user.Connections);
        }

        [Theory, UserManagementTestConventions]
        public void UsersSuccessfullyConnectWhenOtherUserIsInCache(
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            User otherUser,
            ConnectionsController sut)
        {
            cacheTD.Setup(c => c.Find(user.Id.ToString())).Returns((User)null);
            cacheTD.Setup(c => c.Find(otherUser.Id.ToString())).Returns(otherUser);
            repoTD.Setup(r => r.ReadUser(user.Id)).Returns(user);

            var actual = sut.Post(user.Id.ToString(), otherUser.Id.ToString());

            var ok = Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(
                actual);
            Assert.Equal(otherUser, ok.Content);
            repoTD.Verify(r => r.Update(user));
            Assert.Contains(otherUser.Id, user.Connections);
        }

        [Theory, UserManagementTestConventions]
        public void UsersFailToConnectWhenUserIdIsNoInt(
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            string userId,
            User otherUser,
            ConnectionsController sut)
        {
            Assert.False(int.TryParse(userId, out var _));
            cacheTD.Setup(c => c.Find(userId)).Returns((User)null);
            cacheTD.Setup(c => c.Find(otherUser.Id.ToString())).Returns(otherUser);

            var actual = sut.Post(userId, otherUser.Id.ToString());

            Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Theory, UserManagementTestConventions]
        public void UsersFailToConnectWhenOtherUserIdIsNoInt(
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            string otherUserId,
            ConnectionsController sut)
        {
            Assert.False(int.TryParse(otherUserId, out var _));
            cacheTD.Setup(c => c.Find(user.Id.ToString())).Returns(user);
            cacheTD.Setup(c => c.Find(otherUserId)).Returns((User)null);

            var actual = sut.Post(user.Id.ToString(), otherUserId);

            Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Theory, UserManagementTestConventions]
        public void UsersFailToConnectWhenUserIsInNeitherCacheNorRepo(
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            int userId,
            User otherUser,
            ConnectionsController sut)
        {
            cacheTD.Setup(c => c.Find(userId.ToString())).Returns((User)null);
            cacheTD.Setup(c => c.Find(otherUser.Id.ToString())).Returns(otherUser);
            repoTD.Setup(r => r.ReadUser(userId)).Returns((User)null);
            repoTD.Setup(r => r.ReadUser(otherUser.Id)).Returns(otherUser);

            var actual = sut.Post(userId.ToString(), otherUser.Id.ToString());

            Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Theory, UserManagementTestConventions]
        public void UsersFailToConnectWhenOtherUserIsInNeitherCacheNorRepo(
            [Frozen]Mock<IUserCache> cacheTD,
            [Frozen]Mock<IUserRepository> repoTD,
            User user,
            int otherUserId,
            ConnectionsController sut)
        {
            cacheTD.Setup(c => c.Find(user.Id.ToString())).Returns(user);
            cacheTD.Setup(c => c.Find(otherUserId.ToString())).Returns((User)null);
            repoTD.Setup(r => r.ReadUser(user.Id)).Returns(user);
            repoTD.Setup(r => r.ReadUser(otherUserId)).Returns((User)null);

            var actual = sut.Post(user.Id.ToString(), otherUserId.ToString());

            Assert.IsAssignableFrom<BadRequestErrorMessageResult>(actual);
            repoTD.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }
    }
}
