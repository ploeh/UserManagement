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
    }
}
