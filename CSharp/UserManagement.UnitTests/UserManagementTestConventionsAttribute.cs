using AutoFixture;
using AutoFixture.Xunit2;
using System;

namespace Ploeh.Samples.UserManagement.UnitTests
{
    public class UserManagementTestConventionsAttribute : AutoDataAttribute
    {
        public UserManagementTestConventionsAttribute() :
            base(() => new Fixture())
        {
        }
    }
}